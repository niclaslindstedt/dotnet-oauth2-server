#!/usr/bin/env dotnet-script

#load ".common.csx"

Run("docker", $"build -t etimo-id-api .");
