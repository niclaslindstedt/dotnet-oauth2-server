#!/usr/bin/env dotnet-script

#load ".common.csx"

const string JwtSecret = "6f2fb51a-9374-493b-be16-4439ed3212eb";
const string ConnectionString = "Server=localhost;Port=5433;Database=root;User Id=root;Password=root;";

Run("dotnet", $"user-secrets --project Etimo.Id.Api set \"JwtSettings:Secret\" \"{JwtSecret}\"");
Run("dotnet", $"user-secrets --project Etimo.Id.Api set \"ConnectionStrings:EtimoId\" \"{ConnectionString}\"");
