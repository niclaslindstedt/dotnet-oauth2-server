#!/usr/bin/env dotnet-script

#load ".common.csx"

using System.Text.RegularExpressions;

// Get last migration file name (this is done just for the sake of letting the user know what's happening).
var root = GetRootPath();
var files = Directory.GetFiles(Path.Combine(root, "src/Etimo.Id.Data/Migrations"));
var migrationPattern = new Regex(@"/20\d{12}_.+?.cs$");
var designerPattern = new Regex(@".Designer.cs$");
var migrationFiles = files.Where(f => migrationPattern.IsMatch(f) && !designerPattern.IsMatch(f));
var orderedFiles = migrationFiles.OrderByDescending(f => f);
var lastFile = orderedFiles.First();

Console.WriteLine($"Reverting migration '{lastFile}'.");
Run("dotnet", "ef migrations remove --project src/Etimo.Id.Data --startup-project src/Etimo.Id.Api -f --msbuildprojectextensionspath artifacts/obj/Etimo.Id.Api");
