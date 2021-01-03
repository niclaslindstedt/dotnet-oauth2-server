#!/usr/bin/env dotnet-script
#load ".common.csx"

/*
  This script will start etimo-id with the dotnet run
  command. It requires you have .NET 5 SDK installed
  on your host computer. It will start faster than
  if you run it through Docker. This is useful when
  you just need etimo-id to be running, but don't
  really need it to be true to production or be able
  to debug it.
*/

Run("dotnet", $"run --project src/Etimo.Id.Api");
