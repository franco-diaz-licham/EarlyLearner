#!/usr/bin/env pwsh

<#
.SYNOPSIS
Provisions the EarlyLearner Azure infrastructure with Bicep.

.EXAMPLE
./scripts/provision-infra.ps1

.EXAMPLE
./scripts/provision-infra.ps1 -Subscription "00000000-0000-0000-0000-000000000000" -SkipWhatIf

.EXAMPLE
$password = Read-Host "PostgreSQL admin password" -AsSecureString
$key = Read-Host "Encryption key" -AsSecureString
./scripts/provision-infra.ps1 -PostgresAdminPassword $password -EncryptionKey $key
#>

[CmdletBinding()]
param(
    [string]$Subscription,
    [string]$EnvFile = (Join-Path $PSScriptRoot "../infra/.env"),
    [securestring]$PostgresAdminPassword,
    [securestring]$EncryptionKey,
    [switch]$SkipWhatIf,
    [switch]$WhatIfOnly
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

# Load reusable helper functions from the local script module.
Import-Module (Join-Path $PSScriptRoot "InfraHelpers.psm1") -Force

# Validate required local tooling before reading configuration or secrets.
if (-not (Get-Command az -ErrorAction SilentlyContinue)) {
    throw "Azure CLI was not found. Install it from https://learn.microsoft.com/cli/azure/install-azure-cli, then run this script again."
}

# Load environment-driven deployment configuration.
$repoRoot = (Resolve-Path (Join-Path $PSScriptRoot "..")).Path
$envFilePath = if ([IO.Path]::IsPathRooted($EnvFile)) { $EnvFile } else { Join-Path $repoRoot $EnvFile }
Import-DotEnv -Path $envFilePath

# Resolve required deployment values from the environment file and command-line switches.
$subscriptionName = if ([string]::IsNullOrWhiteSpace($Subscription)) { Get-Config -Name "AZURE_SUBSCRIPTION" } else { $Subscription }
$resourceGroupName = Get-Config -Name "AZURE_RESOURCE_GROUP_NAME"
$resourceGroupLocation = Get-Config -Name "AZURE_RESOURCE_GROUP_LOCATION"
$deploymentName = Get-Config -Name "AZURE_DEPLOYMENT_NAME"
$templatePath = Resolve-RepoPath -Path (Get-Config -Name "AZURE_TEMPLATE_FILE") -RepoRoot $repoRoot
$parametersPath = Resolve-RepoPath -Path (Get-Config -Name "AZURE_PARAMETERS_FILE") -RepoRoot $repoRoot
$skipWhatIfEnabled = $SkipWhatIf.IsPresent -or (Get-BoolConfig -Name "AZURE_SKIP_WHAT_IF")
$whatIfOnlyEnabled = $WhatIfOnly.IsPresent -or (Get-BoolConfig -Name "AZURE_WHAT_IF_ONLY")

# Prevent conflicting deployment modes.
if ($skipWhatIfEnabled -and $whatIfOnlyEnabled) {
    throw "Use either -SkipWhatIf or -WhatIfOnly, not both."
}

# Ensure Azure CLI is authenticated before deployment.
$account = $null
try {
    $account = Invoke-AzJson -Arguments @("account", "show", "--output", "json")
} catch {
    Write-Host "Azure CLI is not logged in. Starting az login..." -ForegroundColor Yellow
    Invoke-Az -Arguments @("login", "--output", "none")
    $account = Invoke-AzJson -Arguments @("account", "show", "--output", "json")
}

# Select the target subscription from .env or the explicit script parameter.
if (-not [string]::IsNullOrWhiteSpace($subscriptionName)) {
    Write-Host "Using Azure subscription: $subscriptionName" -ForegroundColor Cyan
    Invoke-Az -Arguments @("account", "set", "--subscription", $subscriptionName)
    $account = Invoke-AzJson -Arguments @("account", "show", "--output", "json")
}

Write-Host "Active subscription: $($account.name) [$($account.id)]" -ForegroundColor Cyan

# Resolve secrets without committing them to the Bicep parameter file.
if ($null -eq $PostgresAdminPassword) {
    $passwordFromEnv = Get-Config -Name "POSTGRES_ADMIN_PASSWORD"
    $PostgresAdminPassword = ConvertTo-SecureString $passwordFromEnv -AsPlainText -Force
}

if ($null -eq $EncryptionKey) {
    $encryptionKeyFromEnv = Get-Config -Name "ENCRYPTION_KEY"
    $EncryptionKey = ConvertTo-SecureString $encryptionKeyFromEnv -AsPlainText -Force
}

$postgresPasswordPlain = ConvertFrom-SecureStringToPlainText $PostgresAdminPassword
$encryptionKeyPlain = ConvertFrom-SecureStringToPlainText $EncryptionKey
if ([string]::IsNullOrWhiteSpace($postgresPasswordPlain)) {
    throw "PostgreSQL admin password is required."
}

if ([string]::IsNullOrWhiteSpace($encryptionKeyPlain)) {
    throw "Encryption key is required."
}

$temporaryParametersFile = Join-Path ([IO.Path]::GetTempPath()) "earlylearner.$([guid]::NewGuid()).parameters.json"

try {
    # Merge committed Bicep parameters with local secret and identity values in a temporary file.
    $parameters = Get-Content $parametersPath -Raw | ConvertFrom-Json -AsHashtable
    $parameters.parameters["postgresAdminPassword"] = @{ value = $postgresPasswordPlain }
    $parameters.parameters["encryptionKey"] = @{ value = $encryptionKeyPlain }
    $parameters.parameters["azureAdInstance"] = @{ value = Get-Config -Name "AZURE_AD_INSTANCE" }
    $parameters.parameters["azureAdTenantId"] = @{ value = Get-Config -Name "AZURE_AD_TENANT_ID" }
    $parameters.parameters["azureAdClientId"] = @{ value = Get-Config -Name "AZURE_AD_CLIENT_ID" }
    $parameters.parameters["frontendEntraClientId"] = @{ value = Get-Config -Name "FRONTEND_ENTRA_CLIENT_ID" }
    $parameters.parameters["frontendEntraAuthority"] = @{ value = Get-Config -Name "FRONTEND_ENTRA_AUTHORITY" }
    $parameters.parameters["frontendEntraApiScope"] = @{ value = Get-Config -Name "FRONTEND_ENTRA_API_SCOPE" }

    # Keep the ACS sender optional so Azure managed domains can supply their generated sender domain.
    $senderAddress = Get-OptionalConfig -Name "COMMUNICATION_SENDER_ADDRESS"
    if (-not [string]::IsNullOrWhiteSpace($senderAddress)) {
        $parameters.parameters["communicationSenderAddress"] = @{ value = $senderAddress }
    }

    $parameters |
        ConvertTo-Json -Depth 20 |
        Set-Content -Path $temporaryParametersFile -Encoding utf8NoBOM

    # Create the resource group before running group-scoped Bicep operations.
    Write-Host "Ensuring resource group exists: $resourceGroupName" -ForegroundColor Cyan
    Invoke-Az -Arguments @(
        "group", "create",
        "--name", $resourceGroupName,
        "--location", $resourceGroupLocation,
        "--output", "none"
    )

    # Preview the deployment unless explicitly skipped.
    if (-not $skipWhatIfEnabled) {
        Write-Host "Running deployment preview..." -ForegroundColor Cyan
        Invoke-Az -Arguments @(
            "deployment", "group", "what-if",
            "--name", $deploymentName,
            "--resource-group", $resourceGroupName,
            "--template-file", $templatePath,
            "--parameters", "@$temporaryParametersFile"
        )
    }

    # Stop after preview when running in what-if-only mode.
    if ($whatIfOnlyEnabled) {
        Write-Host "What-if complete. No resources were provisioned." -ForegroundColor Green
        return
    }

    # Deploy the Bicep template with the generated parameter file.
    Write-Host "Provisioning Azure resources..." -ForegroundColor Cyan
    $deployment = Invoke-AzJson -Arguments @(
        "deployment", "group", "create",
        "--name", $deploymentName,
        "--resource-group", $resourceGroupName,
        "--template-file", $templatePath,
        "--parameters", "@$temporaryParametersFile",
        "--output", "json"
    )

    # Print deployment outputs that need to be copied into GitHub Actions secrets.
    Write-Host "Provisioning complete." -ForegroundColor Green
    Write-Host ""
    Write-Host "GitHub repository secrets to set:" -ForegroundColor Cyan
    $deployment.properties.outputs.githubSecrets.value | Format-List
} finally {
    # Remove the temporary parameter file so passwords and keys are not left on disk.
    if (Test-Path $temporaryParametersFile) {
        Remove-Item -Force $temporaryParametersFile
    }
}
