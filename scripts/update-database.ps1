#!/usr/bin/pwsh

$cwd = Get-Location
Set-Location $PSScriptRoot\.. | Out-Null
dotnet ef database update --project Etimo.Id.Data --startup-project Etimo.Id.Api
Set-Location $cwd | Out-Null
