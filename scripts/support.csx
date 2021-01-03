#!/usr/bin/env dotnet-script

#load ".common.csx"

Run("docker-compose", $"-f docker-compose.yml up -d postgres");
Run("docker-compose", $"-f docker-compose.yml up -d pgadmin");
