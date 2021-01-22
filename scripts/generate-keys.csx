#!/usr/bin/env dotnet-script
#load ".common.csx"

/*
  This script will generate a private and public key
  for asymmetric signing of jwt tokens.
*/

using System.Security.Cryptography;

var rsa = RSA.Create();

string GetPrivateKey() { return Convert.ToBase64String(rsa.ExportRSAPrivateKey()); }
string GetPublicKey() { return Convert.ToBase64String(rsa.ExportRSAPublicKey()); }

string GetKeyOutput(string base64Key, bool privateKey = false)
{
    var keyType = "PUBLIC";
    if (privateKey) { keyType = "PRIVATE"; }

    var splitKey = Split(base64Key, 70);

    var result = new List<string>();
    result.Add($"-----BEGIN {keyType} KEY-----");
    result.AddRange(splitKey);
    result.Add($"-----END {keyType} KEY-----");

    return string.Join("\n", result);
}

var root = GetRootPath();
var privateKey = GetPrivateKey();
var publicKey = GetPublicKey();

if (Args.Count() > 1 && Args[0] == "--output")
{
    if (Args[1] == "user-secrets")
    {
        Run("dotnet", $"user-secrets --project {root}/src/Etimo.Id.Api init");
        Run("dotnet", $"user-secrets --project {root}/src/Etimo.Id.Api set EtimoIdSettings:PrivateKey {privateKey}");
        Run("dotnet", $"user-secrets --project {root}/src/Etimo.Id.Api set EtimoIdSettings:PublicKey {publicKey}");
    }
    else if (Args[1] == "raw")
    {
        Console.WriteLine($"PRIVATE KEY: {privateKey}");
        Console.WriteLine($"PUBLIC KEY: {publicKey}");
    }
    else if (Args[1] == "file")
    {
        var privateKeyFile = $"{root}/key_rsa";
        var publicKeyFile = $"{root}/key_rsa.pub";
        File.WriteAllText(privateKeyFile, GetKeyOutput(privateKey, true));
        File.WriteAllText(publicKeyFile, GetKeyOutput(publicKey, true));

        Console.WriteLine($"Wrote private key to {privateKeyFile}");
        Console.WriteLine($"Wrote public key to {publicKeyFile}");
    }
}
else
{
    Console.WriteLine(GetKeyOutput(privateKey, true));
    Console.WriteLine(GetKeyOutput(publicKey));
}
