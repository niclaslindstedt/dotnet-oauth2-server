#!/usr/bin/env dotnet-script

#load ".common.csx"

Run("jb", $"cleanupcode etimo-id.sln --toolset=16.0 --verbosity=WARN --exclude=**/Migrations/*");
