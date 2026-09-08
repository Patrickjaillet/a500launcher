using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace A500Launcher.Services;

public sealed record ImportResult(int Added, int Skipped);

public static class AdfImporter
{
    public static ImportResult Scan(string folder, ICollection<string> knownPaths)
    {
        if (!Directory.Exists(folder))
        {
            return new ImportResult(0, 0);
        }

        var knownHashes = new HashSet<string>(
            knownPaths.Where(File.Exists).Select(Hash),
            StringComparer.Ordinal);

        var added = 0;
        var skipped = 0;

        foreach (var file in Directory.EnumerateFiles(folder, "*.adf", SearchOption.AllDirectories))
        {
            var hash = Hash(file);
            if (!knownHashes.Add(hash))
            {
                skipped++;
                continue;
            }

            if (!knownPaths.Contains(file))
            {
                knownPaths.Add(file);
                added++;
            }
        }

        return new ImportResult(added, skipped);
    }

    private static string Hash(string path)
    {
        using var stream = File.OpenRead(path);
        return Convert.ToHexString(SHA256.HashData(stream));
    }
}
