#!/usr/bin/pwsh

$cwd = Get-Location | Out-Null
Set-Location $PSScriptRoot/../Etimo.Id.Api
dotnet run
Set-Location $cwd | Out-Null
