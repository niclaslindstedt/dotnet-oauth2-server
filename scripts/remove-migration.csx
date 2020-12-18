#!/usr/bin/env dotnet-script

#load ".common.csx"

Run("dotnet", $"ef migrations remove --project Etimo.Id.Data --startup-project Etimo.Id.Api");
