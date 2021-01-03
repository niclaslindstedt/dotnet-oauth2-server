#!/usr/bin/env dotnet-script
#load ".common.csx"

/*
  This script will cleanup the code and format it
  according to the rules laid out in .editorconfig.
  It will by default only format modified files.
  If you wish to format all files, run it with the
  --all argument.
*/

var formatAll = Args.Any(a => a == "--all");

var stagedFiles = RunCatchOutput("git", "diff --name-only --cached").Split("\n");
var modifiedFiles = RunCatchOutput("git", "diff --name-only").Split("\n");
var untrackedFiles = RunCatchOutput("git", "ls-files --others --exclude-standard").Split("\n");
var changedFiles = stagedFiles.Concat(modifiedFiles).Concat(untrackedFiles).Where(f => !f.Contains("/Migrations/"));

if (!formatAll && !changedFiles.Any(f => f.EndsWith(".cs")))
{
    Console.WriteLine("Skipping cleanup -- no .cs files have been modified. Use --all to format all.");
    Environment.Exit(0);
}

var filesToFormat = changedFiles.Where(f => f.EndsWith(".cs"));
var filesList = string.Join(";", filesToFormat).TrimEnd(';');
var include = $"--include=\"{filesList}\"";
if (!formatAll)
{
    var fileCount = filesToFormat.Count();
    Console.WriteLine($"Formatting {fileCount} file{(fileCount > 1 ? "s" : "")}");
}
else
{
    Console.WriteLine("Formatting all files");
    include = "";
}

Run("jb", $"cleanupcode etimo-id.sln -p=\"Built-in: Full Cleanup\" --toolset=16.0 {include} --exclude=**/Migrations/*");
