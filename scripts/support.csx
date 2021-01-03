#!/usr/bin/env dotnet-script
#load ".common.csx"

/*
  This script will start only the supportive services
  in the stack. I.e. the database and the database
  administration tool. Useful when you plan on running
  etimo-id outside Docker, e.g. for debugging purposes.
*/

Run("docker-compose", $"-f docker-compose.yml up -d postgres");
Run("docker-compose", $"-f docker-compose.yml up -d pgadmin");
