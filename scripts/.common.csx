#!/usr/bin/env dotnet-script

using System.Diagnostics;

string GetRootPath([System.Runtime.CompilerServices.CallerFilePath] string fileName = null)
{
    var scriptDir = Path.GetDirectoryName(fileName);
    return Path.GetFullPath(Path.Combine(scriptDir, ".."));
}

int Run(string executable, string arguments = "")
{
    var process = new Process {
        StartInfo = new ProcessStartInfo
        {
            WorkingDirectory = GetRootPath(),
            FileName = executable,
            Arguments = arguments
        }
    };

    process.Start();
    process.WaitForExit();

    return process.ExitCode;
}

string RunCatchOutput(string executable, string arguments = "")
{
    var process = new Process {
        StartInfo = new ProcessStartInfo
        {
            WorkingDirectory = GetRootPath(),
            FileName = executable,
            Arguments = arguments,
            RedirectStandardOutput = true
        }
    };

    process.Start();
    string output = process.StandardOutput.ReadToEnd();
    process.WaitForExit();

    return output;
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

void PrependToFile(string path, string content)
{
    var rootDir = GetRootPath();
    var fullPath = Path.Combine(rootDir, path);

    StreamWriter sw;
    if (File.Exists(path))
    {
        var existingContent = File.ReadAllText(path);
        sw = File.CreateText(path);
        sw.Write(content);
        sw.Write(existingContent);
    }
    else
    {
        sw = File.CreateText(path);
        sw.Write(content);
    }

    sw.Dispose();
}

List<string> Split(string str, int chunkSize)
{
    var result = new List<string>();
    var strCopy = new string(str);
    while (strCopy.Length > chunkSize)
    {
        result.Add(strCopy.Substring(0, chunkSize));
        strCopy = strCopy.Substring(chunkSize, strCopy.Length - chunkSize);
    }
    result.Add(strCopy);

    return result;
}
