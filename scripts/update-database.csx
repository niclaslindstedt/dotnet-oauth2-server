#!/usr/bin/env dotnet-script

#load ".common.csx"

Run("dotnet", $"ef database update --project Etimo.Id.Data --startup-project Etimo.Id.Api");
