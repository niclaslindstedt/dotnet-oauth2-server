#!/usr/bin/env dotnet-script
#load ".common.csx"

/*
  This script will follow the logs of the etimo-id
  container. This is the only container of interest
  in the stack, and is safe to CTRL+C out of.
*/

Run("docker-compose", "logs --tail 100 -f etimo-id");
