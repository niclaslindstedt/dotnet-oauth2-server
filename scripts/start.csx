#!/usr/bin/env dotnet-script
#load ".common.csx"

/*
  This script will build, start and follow the logs
  of the entire stack needed to run etimo-id. It targets
  the prod build stage in the Dockerfile, which will be
  close to what will be running in production. It is safe
  to CTRL+C out of this -- it will be kept running in the
  background.
*/

Run("docker-compose", $"-f docker-compose.yml -f docker-compose.dev.yml build --parallel");
Run("docker-compose", $"-f docker-compose.yml -f docker-compose.dev.yml up -d");
Run("docker-compose", $"-f docker-compose.yml -f docker-compose.dev.yml logs -f --tail 100 etimo-id");
