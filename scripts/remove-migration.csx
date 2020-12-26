#!/usr/bin/env dotnet-script

#load ".common.csx"

Run("dotnet", $"ef migrations remove --project src/Etimo.Id.Data --startup-project src/Etimo.Id.Api");
