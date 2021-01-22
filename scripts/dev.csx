#!/usr/bin/env dotnet-script
#load ".common.csx"

/*
  This script will build, start and follow the logs
  of the entire stack needed to run etimo-id. It targets
  the dev build stage in the Dockerfile, which will enable
  hot reload when you update the code. It is safe to
  CTRL+C out of this -- it will be kept running in the
  background.
*/

Run("docker-compose", $"-f docker-compose.yml -f docker-compose.db.yml -f docker-compose.local.yml build --parallel");
Run("docker-compose", $"-f docker-compose.yml -f docker-compose.db.yml -f docker-compose.local.yml up -d");
Run("docker-compose", $"-f docker-compose.yml -f docker-compose.db.yml -f docker-compose.local.yml logs -f --tail 100 etimo-id-api etimo-id-web");
