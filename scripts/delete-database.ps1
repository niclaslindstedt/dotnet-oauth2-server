#!/usr/bin/pwsh

$cwd = Get-Location
Set-Location $PSScriptRoot\.. | Out-Null
docker rm -f etimo-id-db
docker volume rm -f etimo-id_db-data
docker-compose up -d
Set-Location $cwd | Out-Null

Write-Host "Now run update-database.ps1 to migrate the database to the latest version."
