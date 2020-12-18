#!/usr/bin/env dotnet-script

#load ".common.csx"

Run("docker", "rm -f etimo-id-db");
Run("docker", "volume rm -f etimo-id_db-data");
Run("docker-compose", "up -d");

Console.WriteLine("Now run update-database.csx to migrate the database to the latest version.");
