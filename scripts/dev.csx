#!/usr/bin/env dotnet-script

#load ".common.csx"

Run("docker-compose", $"-f docker-compose.yml -f docker-compose.local.yml build --parallel");
Run("docker-compose", $"-f docker-compose.yml -f docker-compose.local.yml up -d");
Run("docker-compose", $"-f docker-compose.yml -f docker-compose.local.yml logs -f --tail 100 etimo-id");
