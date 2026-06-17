#!/usr/bin/env pwsh

<#
.SYNOPSIS
Waits for PostgreSQL and runs the EarlyLearner seed script.

.EXAMPLE
./scripts/init-db.ps1
#>

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

# Use the database connection provided by Docker Compose.
$seedFile = $env:SEED_FILE

# Check local prerequisites before waiting on PostgreSQL.
if (-not (Get-Command psql -ErrorAction SilentlyContinue)) {
    Write-Host "🌱 ⚠️ psql was not found. Install PostgreSQL client tools before running this script." -ForegroundColor Red
    exit 1
}

if (-not (Test-Path -LiteralPath $seedFile -PathType Leaf)) {
    Write-Host "🌱 ⚠️ Seed file was not found: $seedFile" -ForegroundColor Red
    exit 1
}

# Build the connection used by psql.
# psql reads PGPASSWORD from its process environment and sends it during PostgreSQL authentication.
$env:PGPASSWORD = $env:POSTGRES_PASSWORD
$connection = "host=$env:POSTGRES_HOST port=$env:POSTGRES_PORT dbname=$env:POSTGRES_DB user=$env:POSTGRES_USER"

Write-Host "Using Docker database target." -ForegroundColor Cyan
Write-Host "Waiting for PostgreSQL at $env:POSTGRES_HOST`:$env:POSTGRES_PORT/$env:POSTGRES_DB..." -ForegroundColor Yellow

# Wait until PostgreSQL accepts connections.
while ($true) {
    psql $connection -v ON_ERROR_STOP=1 -c "select 1" *> $null
    if ($LASTEXITCODE -eq 0) {
        break
    }

    Write-Host "PostgreSQL is not ready. Retrying in 2 seconds..." -ForegroundColor DarkYellow
    Start-Sleep -Seconds 2
}

# Run the single EarlyLearner seed file.
Write-Host "PostgreSQL is ready." -ForegroundColor Green
Write-Host "Running seed file: $seedFile" -ForegroundColor Cyan

psql $connection -v ON_ERROR_STOP=1 -f $seedFile
if ($LASTEXITCODE -ne 0) {
    Write-Host "🌱 ❌ Seeding failed. psql returned exit code $LASTEXITCODE." -ForegroundColor Red
    exit $LASTEXITCODE
}

Write-Host "🌱 ✅ Seeding complete." -ForegroundColor Green
