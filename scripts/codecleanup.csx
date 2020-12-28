#!/usr/bin/env dotnet-script

#load ".common.csx"

Run("jb", $"cleanupcode etimo-id.sln --verbosity=WARN --exclude=**/Migrations/*");
