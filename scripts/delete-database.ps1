#!/usr/bin/pwsh

$cwd = Get-Location
Set-Location $PSScriptRoot\.. | Out-Null
docker rm -f etimo-id-db
docker volume rm -f etimo-id_db-data
docker-compose up -d
Set-Location $cwd | Out-Null
