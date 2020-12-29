#!/usr/bin/env dotnet-script

#load ".common.csx"

Run("docker", $"stop etimo-id-api");
