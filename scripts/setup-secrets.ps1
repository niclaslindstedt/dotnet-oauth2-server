#!/usr/bin/pwsh

$jwtSecret        = "6f2fb51a-9374-493b-be16-4439ed3212eb"
$connectionString = "Server=localhost;Port=5433;Database=root;User Id=root;Password=root;"

$cwd = Get-Location | Out-Null
Set-Location $PSScriptRoot/../Etimo.Id.Api
dotnet user-secrets set "JwtSettings:Secret" "$jwtSecret"
dotnet user-secrets set "ConnectionStrings:EtimoId" "$connectionString"
Set-Location $cwd | Out-Null
