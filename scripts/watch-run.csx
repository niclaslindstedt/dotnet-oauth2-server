#!/usr/bin/env dotnet-script
#load ".common.csx"

/*
  This script will run the code and watch for code changes,
  once the code changes, it will rebuild and restart (hot reload).
*/

Run("dotnet", $"watch --project src/Etimo.Id.Api run");
