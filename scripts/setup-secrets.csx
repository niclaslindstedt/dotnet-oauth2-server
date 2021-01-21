#!/usr/bin/env dotnet-script
#load ".common.csx"

/*
  This script will setup secrets that will let you
  work on etimo-id. Since they have been committed to
  GitHub, it is not recommended to use these for any
  other purpose than local debugging. DO NOT use this
  script in a production environment.
*/

const string SymmetricKey = "6f2fb51a-9374-493b-be16-4439ed3212eb";
const string ConnectionString = "Server=localhost;Port=5433;Database=root;User Id=root;Password=root;";

Run("dotnet", $"user-secrets --project src/Etimo.Id.Api init");
Run("dotnet", $"user-secrets --project src/Etimo.Id.Api set \"EtimoIdSettings:Secret\" \"{SymmetricKey}\"");
Run("dotnet", $"user-secrets --project src/Etimo.Id.Api set \"ConnectionStrings:EtimoId\" \"{ConnectionString}\"");
