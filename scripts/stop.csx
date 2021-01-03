#!/usr/bin/env dotnet-script
#load ".common.csx"

/*
  This script will stop only the etimo-id container.
  This is useful when you want to run etimo-id outside
  Docker, e.g. in a debug session, and need the Docker
  container to stop listening on your port.
*/

Run("docker-compose", $"stop etimo-id");
