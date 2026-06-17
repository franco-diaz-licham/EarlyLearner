#!/usr/bin/env pwsh

<#
.SYNOPSIS
Stops and rebuilds the EarlyLearner Docker Compose environment.

.EXAMPLE
./scripts/compose.ps1
#>

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

# Resolve the compose file from the repository root.
$scriptDirectory = Split-Path -Parent $PSCommandPath
$repoRoot = Split-Path -Parent $scriptDirectory
$composeFile = Join-Path $repoRoot "docker/docker-compose.yml"

if (-not (Test-Path -LiteralPath $composeFile -PathType Leaf)) {
    Write-Host "🐳 ⚠️ Compose file was not found: $composeFile" -ForegroundColor Red
    exit 1
}

$composeArgs = @("-f", $composeFile, "--profile", "core")

# Stop all services first so the environment starts from a clean compose state.
Write-Host "Stopping EarlyLearner Docker environment..." -ForegroundColor Yellow
docker compose @composeArgs down
if ($LASTEXITCODE -ne 0) {
    Write-Host "🐳 ❌ Docker Compose down failed with exit code $LASTEXITCODE." -ForegroundColor Red
    exit $LASTEXITCODE
}

# Rebuild and start every service in the core profile.
Write-Host "Starting EarlyLearner Docker environment..." -ForegroundColor Cyan
docker compose @composeArgs up -d --build
if ($LASTEXITCODE -ne 0) {
    Write-Host "🐳 ❌ Docker Compose up failed with exit code $LASTEXITCODE." -ForegroundColor Red
    exit $LASTEXITCODE
}

Write-Host "🐳 ✅ Docker environment is ready." -ForegroundColor Green
