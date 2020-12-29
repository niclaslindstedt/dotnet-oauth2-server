#!/usr/bin/env dotnet-script

#load ".common.csx"

Run("docker-compose", $"stop etimo-id");
