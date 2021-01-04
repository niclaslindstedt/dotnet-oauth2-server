#!/usr/bin/env dotnet-script
#load ".common.csx"

/*
  This script will revert a migration. If the migration
  has been applied to the database, it will revert the
  database to the previous migration. This is useful
  if you've already added a migration, but need to make
  further updates before committing the code, and you
  don't want several migrations for your changes.
*/

using System.Text.RegularExpressions;

// Get last migration file name (this is done just for the sake of letting the user know what's happening).
var root = GetRootPath();
var files = Directory.GetFiles(Path.Combine(root, "src/Etimo.Id.Data/Migrations"));
var migrationPattern = new Regex(@"[/\\]20\d{12}_.+?.cs$");
var designerPattern = new Regex(@".Designer.cs$");
var migrationFiles = files.Where(f => migrationPattern.IsMatch(f) && !designerPattern.IsMatch(f));
var orderedFiles = migrationFiles.OrderByDescending(f => f);
var lastFile = orderedFiles.First();

Console.WriteLine($"Reverting migration '{lastFile}'.");
Run("dotnet", "ef migrations remove --project src/Etimo.Id.Data --startup-project src/Etimo.Id.Api -f --msbuildprojectextensionspath artifacts/obj/Etimo.Id.Api");
