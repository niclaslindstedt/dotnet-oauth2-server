#!/usr/bin/env dotnet-script
#load ".common.csx"

/*
  This script creates a migration file to be used
  to update the database. I.e. if you've changed the
  database entities, you want to sync this so your
  database is up to date, otherwise the app will fail.
*/

if (Args.Count == 0)
{
    Console.WriteLine("You need to specify a migration name");
    Environment.Exit(1);
}

var exitCode = Run("dotnet", $"ef migrations add {Args[0]} --project src/Etimo.Id.Data --startup-project src/Etimo.Id.Api --msbuildprojectextensionspath artifacts/obj/Etimo.Id.Api");

if (exitCode == 0)
{
    Console.WriteLine("Now run update-database.csx to migrate the database to the latest version.");
}
