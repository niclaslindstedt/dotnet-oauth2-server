#!/usr/bin/env dotnet-script
#load ".common.csx"

/*
  This script will delete the database docker volume
  and the database docker container. It will then
  recreate it, so you start with a fresh running
  database container.
*/

Run("docker", "rm -f etimo-id-db");
Run("docker", "volume rm -f etimo-id_db-data");
Run("docker-compose", "up -d");

Console.WriteLine("Now run update-database.csx to migrate the database to the latest version.");
