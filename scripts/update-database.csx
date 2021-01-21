#!/usr/bin/env dotnet-script
#load ".common.csx"

/*
  This script will update the database to the latest
  migration. You need to run this after running the
  add-migration.csx script. You also need to run this
  if it's your first time starting the application.
  If etimo-id has automatically created a database for
  you when you started it. Keep it running and run the
  'delete-database.csx' script, and run this script again.
*/

Run("dotnet", $"ef database update --project src/Etimo.Id.Data --startup-project src/Etimo.Id.Api --msbuildprojectextensionspath artifacts/obj/Etimo.Id.Data");
Run("dotnet", $"ef database update --project src/Etimo.Id.Data --startup-project src/Etimo.Id.Api --msbuildprojectextensionspath artifacts/obj/Etimo.Id.Api");
