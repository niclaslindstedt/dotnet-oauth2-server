#!/usr/bin/env dotnet-script

#load ".common.csx"

Run("dotnet", $"ef migrations add {Args[0]} --project src/Etimo.Id.Data --startup-project src/Etimo.Id.Api --msbuildprojectextensionspath artifacts/obj/Etimo.Id.Api");

Console.WriteLine("Now run update-database.csx to migrate the database to the latest version.");
