#!/usr/bin/env dotnet-script
#load ".common.csx"

/*
  This script will install the 'code-cleanup.sh' git
  hook in the .git/hooks/ folder. If the 'pre-commit'
  script has already been created, it will append a
  line that executes the 'code-cleanup.sh' script.
*/

var root = GetRootPath();
var preCommitHookFile = $"{root}/.git/hooks/pre-commit";
var hookFileName = "code-cleanup.sh";
var hookPath = $"{root}/.git/hooks/{hookFileName}";
var content = $"$(dirname $0)/{hookFileName}";

var shebangExists = false;
var hookInstalled = false;
if (File.Exists(preCommitHookFile))
{
    var existingContent = File.ReadAllLines(preCommitHookFile);
    if (existingContent.Any())
    {
        hookInstalled = existingContent.Any(l => l == content);
        shebangExists = existingContent.First().StartsWith(@"#!");
    }
}

if (!shebangExists)
{
    Console.WriteLine("* Writing shebang to 'pre-commit' file");
    PrependToFile(preCommitHookFile, "#!/bin/sh\n");
}

if (!hookInstalled)
{
    Console.WriteLine("* Adding hook script execution to 'pre-commit' file");
    AppendToFile(preCommitHookFile, content);
}

if (!File.Exists(hookPath))
{
    Console.WriteLine("* Copying hook script to ./git/hooks/");
    File.Copy($"{root}/scripts/{hookFileName}", hookPath);
}

if (File.Exists("/bin/chmod"))
{
    Console.WriteLine("* Setting execution permissions");
    Run("chmod", $"+x {preCommitHookFile}");
    Run("chmod", $"+x {hookPath}");
}

Console.WriteLine("\nPre-commit hook successfully installed");