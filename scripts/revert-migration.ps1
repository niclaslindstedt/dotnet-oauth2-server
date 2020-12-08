#!/usr/bin/pwsh

$cwd = Get-Location
Set-Location $PSScriptRoot\.. | Out-Null

$last = (Get-ChildItem "Etimo.Id.Data\Migrations" | `
    Where-Object Name -match "^20\d{12}_.+?.cs$" | `
    Where-Object Name -notmatch ".Designer.cs$" | `
    Sort-Object -Descending | `
    Select-Object -First 1) -replace ".cs$",""

Write-Host "Reverting migration '$last'."
dotnet ef migrations remove --project Etimo.Id.Data --startup-project Etimo.Id.Api -f

Set-Location $cwd | Out-Null
