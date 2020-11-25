$cwd = Get-Location
Set-Location $PSScriptRoot\.. | Out-Null
dotnet ef migrations remove --project Etimo.Id.Data --startup-project Etimo.Id.Api
Set-Location $cwd | Out-Null
