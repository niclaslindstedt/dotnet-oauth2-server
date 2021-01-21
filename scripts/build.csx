#!/usr/bin/env dotnet-script
#load ".common.csx"

/*
  This script will build etimo-id Docker images.
  Since it will build the entire Dockerfiles, it will
  also run tests. It's a good way of knowing everything
  will build once deployed. It will not, however, run
  the application. So you will have to run the start.csx
  script to ensure it actually runs.
*/

Run("docker-compose", $"build");
