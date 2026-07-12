param(
    [string]$EmulatorEndpoint = "https://localhost:8081",
    [string]$CertificatePath = "$PSScriptRoot\cosmos-emulator.pem"
)

$ErrorActionPreference = "Stop"

$certificateUrl = "$EmulatorEndpoint/_explorer/emulator.pem"
$certificateStore = "Cert:\CurrentUser\Root"

Write-Host "Downloading Cosmos DB emulator certificate from $certificateUrl"
& curl.exe -k -fsSL $certificateUrl -o $CertificatePath

if (-not (Test-Path -LiteralPath $CertificatePath)) {
    throw "Certificate was not downloaded to $CertificatePath."
}

$certificate = New-Object System.Security.Cryptography.X509Certificates.X509Certificate2($CertificatePath)
$existingCertificate = Get-ChildItem -Path $certificateStore | Where-Object { $_.Thumbprint -eq $certificate.Thumbprint }

if ($existingCertificate) {
    Write-Host "Cosmos DB emulator certificate is already trusted."
    Write-Host "Thumbprint: $($certificate.Thumbprint)"
    return
}

Write-Host "Importing Cosmos DB emulator certificate into $certificateStore"
Import-Certificate -FilePath $CertificatePath -CertStoreLocation $certificateStore | Out-Null

$trustedCertificate = Get-ChildItem -Path $certificateStore | Where-Object { $_.Thumbprint -eq $certificate.Thumbprint }
if (-not $trustedCertificate) {
    throw "Certificate import completed, but the certificate was not found in $certificateStore."
}

Write-Host "Cosmos DB emulator certificate trusted successfully."
Write-Host "Thumbprint: $($certificate.Thumbprint)"
Write-Host "Restart VS Code or any tool that already cached certificate trust state."
