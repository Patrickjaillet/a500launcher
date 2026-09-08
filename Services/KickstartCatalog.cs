using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Hashing;

namespace A500Launcher.Services;

public static class KickstartCatalog
{
    public static IReadOnlyDictionary<uint, string> KnownCrcs => KnownRoms;

    private static readonly Dictionary<uint, string> KnownRoms = new()
    {
        [0x11FE471Fu] = "Kickstart 1.2 (33.166)",
        [0xA0DA88E6u] = "Kickstart 1.2 (33.180)",
        [0xC4F0F55Fu] = "Kickstart 1.3 (34.005)",
        [0xE0F37258u] = "Kickstart 1.3 (34.005, A3000)",
        [0x9ED783D0u] = "Kickstart 2.04 (37.175)",
        [0x83028FB5u] = "Kickstart 3.1 (40.063, A600)",
    };

    public static string? Identify(string path)
    {
        if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
        {
            return null;
        }

        try
        {
            var bytes = File.ReadAllBytes(path);
            var crc = BitConverter.ToUInt32(Crc32.Hash(bytes));
            return KnownRoms.TryGetValue(crc, out var name) ? name : null;
        }
        catch (IOException)
        {
            return null;
        }
    }

    public static bool IsA500Kickstart(string path)
    {
        var name = Identify(path);
        return name is not null && (name.Contains("1.2") || name.Contains("1.3"));
    }
}
