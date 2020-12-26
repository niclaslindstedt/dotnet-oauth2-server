#!/usr/bin/env dotnet-script

#load ".common.csx"

Run("docker", "exec -it etimo-id-api /bin/bash");
