using System;
using System.IO;
using System.Linq;
using A500Launcher.Models;

namespace A500Launcher.Services;

public static class PortableLayout
{
    public static string BaseDir => AppContext.BaseDirectory;

    public static string WinUaeDir => Path.Combine(BaseDir, "winuae");

    public static string BiosDir => Path.Combine(BaseDir, "bios");

    public static string RomsDir => Path.Combine(BaseDir, "roms");

    public static void EnsureDirectories()
    {
        Directory.CreateDirectory(WinUaeDir);
        Directory.CreateDirectory(BiosDir);
        Directory.CreateDirectory(RomsDir);
    }

    public static string? FindWinUaeExe()
    {
        foreach (var name in new[] { "winuae64.exe", "winuae.exe" })
        {
            var candidate = Path.Combine(WinUaeDir, name);
            if (File.Exists(candidate))
            {
                return candidate;
            }
        }

        return null;
    }

    public static string? FindKickstartRom()
    {
        if (!Directory.Exists(BiosDir))
        {
            return null;
        }

        var candidates = Directory
            .EnumerateFiles(BiosDir)
            .Where(file => file.EndsWith(".rom", StringComparison.OrdinalIgnoreCase)
                        || file.EndsWith(".bin", StringComparison.OrdinalIgnoreCase))
            .OrderBy(file => file, StringComparer.OrdinalIgnoreCase)
            .ToList();

        var preferred = candidates.FirstOrDefault(file =>
        {
            var name = Path.GetFileName(file).ToLowerInvariant();
            return name.Contains("kick")
                || name.Contains("1.3")
                || name.Contains("1.2")
                || name.Contains("13")
                || name.Contains("12");
        });

        return preferred ?? candidates.FirstOrDefault();
    }

    public static bool ApplyDefaults(AppSettings settings)
    {
        EnsureDirectories();
        var changed = false;

        if (string.IsNullOrWhiteSpace(settings.WinUaeExePath) || !File.Exists(settings.WinUaeExePath))
        {
            var exe = FindWinUaeExe();
            if (exe is not null)
            {
                settings.WinUaeExePath = exe;
                changed = true;
            }
        }

        if (string.IsNullOrWhiteSpace(settings.KickstartRomPath) || !File.Exists(settings.KickstartRomPath))
        {
            var rom = FindKickstartRom();
            if (rom is not null)
            {
                settings.KickstartRomPath = rom;
                changed = true;
            }
        }

        if (string.IsNullOrWhiteSpace(settings.DefaultAdfFolder) || !Directory.Exists(settings.DefaultAdfFolder))
        {
            settings.DefaultAdfFolder = RomsDir;
            changed = true;
        }

        return changed;
    }
}
