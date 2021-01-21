#!/usr/bin/env dotnet-script
#load ".common.csx"

/*
  This script will setup secrets that will let you
  work on etimo-id. Since they have been committed to
  GitHub, it is not recommended to use these for any
  other purpose than local debugging. DO NOT use this
  script in a production environment.
*/

const string JwtSecret = "6f2fb51a-9374-493b-be16-4439ed3212eb";
const string ClientId = "11111111-1111-1111-1111-111111111111";
const string ClientSecret = "etimo";
const string ConnectionString = "Server=localhost;Port=5433;Database=root;User Id=root;Password=root;";

Run("dotnet", $"user-secrets --project src/Etimo.Id.Api init");
Run("dotnet", $"user-secrets --project src/Etimo.Id.Api set \"EtimoIdSettings:Secret\" \"{JwtSecret}\"");
Run("dotnet", $"user-secrets --project src/Etimo.Id.Api set \"ConnectionStrings:EtimoId\" \"{ConnectionString}\"");

Run("dotnet", $"user-secrets --project src/Etimo.Id.Web init");
Run("dotnet", $"user-secrets --project src/Etimo.Id.Web set \"EtimoIdSettings:Secret\" \"{JwtSecret}\"");
Run("dotnet", $"user-secrets --project src/Etimo.Id.Api set \"EtimoIdSettings:ClientId\" \"{ClientId}\"");
Run("dotnet", $"user-secrets --project src/Etimo.Id.Api set \"EtimoIdSettings:ClientSecret\" \"{ClientSecret}\"");
