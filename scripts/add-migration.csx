#!/usr/bin/env dotnet-script

#load ".common.csx"

Run("dotnet", $"ef migrations add {Args[0]} --project Etimo.Id.Data --startup-project Etimo.Id.Api");
