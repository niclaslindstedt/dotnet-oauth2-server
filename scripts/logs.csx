#!/usr/bin/env dotnet-script

#load ".common.csx"

Run("docker-compose", "logs --tail 100 -f etimo-id");
