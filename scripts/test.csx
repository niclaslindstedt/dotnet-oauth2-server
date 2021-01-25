#!/usr/bin/env dotnet-script
#load ".common.csx"

/*
  This script will run the tests just like the
  GitHub workflow does. So you'll know if it will
  pass or not. It requires you have .NET 5 SDK
  installed.
*/

Run("dotnet", $"restore");
Run("dotnet", $"build etimo-id.sln --configuration Release --no-restore");
Run("dotnet", $"test etimo-id.sln --configuration Release --no-restore --no-build --collect:\"XPlat Code Coverage\" --results-directory ./testresults");
