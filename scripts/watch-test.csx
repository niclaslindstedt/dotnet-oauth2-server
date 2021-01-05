#!/usr/bin/env dotnet-script
#load ".common.csx"

/*
  This script will run tests and watch for code changes,
  once the code changes, it will rebuild and retest (hot reload).
*/

Run("dotnet", $"watch --project src/Etimo.Id.Tests test");
