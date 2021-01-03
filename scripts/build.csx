#!/usr/bin/env dotnet-script
#load ".common.csx"

/*
  This script will build an etimo-id Docker image.
  Since it will build the entire Dockerfile, it will
  also run tests. It's a good way of knowing everything
  will build once deployed. It will not, however, run
  the application. So you will have to run the start.csx
  script to ensure it actually runs.
*/

Run("docker", $"build -t etimo-id-api .");
