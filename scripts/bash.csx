#!/usr/bin/env dotnet-script

#load ".common.csx"

Run("docker-compose", "exec etimo-id /bin/bash");
