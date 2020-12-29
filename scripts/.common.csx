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

void AppendToFile(string path, string content)
{
    var rootDir = GetRootPath();
    var fullPath = Path.Combine(rootDir, path);

    StreamWriter sw;
    if (!File.Exists(path))
    {
        sw = File.CreateText(path);
    }
    else
    {
        sw = File.AppendText(path);
    }

    sw.Write(content);
    sw.Dispose();
}