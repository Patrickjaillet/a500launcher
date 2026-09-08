using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace A500Launcher.Services;

public static class RecentFiles
{
    public const int Limit = 10;

    public static void Add(List<string> recent, string path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            return;
        }

        recent.RemoveAll(entry => string.Equals(entry, path, StringComparison.OrdinalIgnoreCase));
        recent.Insert(0, path);

        while (recent.Count > Limit)
        {
            recent.RemoveAt(recent.Count - 1);
        }
    }

    public static IReadOnlyList<string> Existing(IEnumerable<string> recent)
    {
        return recent.Where(File.Exists).ToList();
    }
}
