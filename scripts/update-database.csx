#!/usr/bin/env dotnet-script

#load ".common.csx"

Run("dotnet", $"ef database update --project src/Etimo.Id.Data --startup-project src/Etimo.Id.Api");
