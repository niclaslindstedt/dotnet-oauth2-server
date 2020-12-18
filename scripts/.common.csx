#!/usr/bin/env dotnet-script

using System.Diagnostics;

string GetRootPath([System.Runtime.CompilerServices.CallerFilePath] string fileName = null)
{
    var scriptDir = Path.GetDirectoryName(fileName);
    return Path.GetFullPath(Path.Combine(scriptDir, ".."));
}

void Run(string executable, string arguments = null)
{
    var startInfo = new ProcessStartInfo
    {
        WorkingDirectory = GetRootPath(),
        FileName = executable,
        Arguments = arguments ?? ""
    };

    Process.Start(startInfo).WaitForExit();
}
