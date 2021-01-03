#!/usr/bin/env dotnet-script
#load ".common.csx"

/*
  This script will let you start a bash session
  inside an already running container.
*/

Run("docker-compose", "exec etimo-id /bin/bash");
