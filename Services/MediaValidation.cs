using System;
using System.IO;

namespace A500Launcher.Services;

public enum MediaStatus
{
    Missing,
    Recognised,
    NonStandard,
}

public sealed record MediaCheck(MediaStatus Status, string DetailKey);

public static class MediaValidation
{
    private const long KickstartMin = 256 * 1024;
    private const long KickstartMax = 512 * 1024;

    private static readonly long[] StandardAdfSizes = { 901_120, 912_384, 983_040 };

    public static MediaCheck CheckKickstart(string? path)
    {
        if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
        {
            return new MediaCheck(MediaStatus.Missing, "validation.rom.missing");
        }

        var identified = KickstartCatalog.Identify(path);
        if (identified is not null)
        {
            return KickstartCatalog.IsA500Kickstart(path)
                ? new MediaCheck(MediaStatus.Recognised, "validation.rom.ok")
                : new MediaCheck(MediaStatus.NonStandard, "validation.rom.notA500");
        }

        var length = new FileInfo(path).Length;
        if (length is >= KickstartMin and <= KickstartMax && (length & (length - 1)) == 0)
        {
            return new MediaCheck(MediaStatus.Recognised, "validation.rom.ok");
        }

        return new MediaCheck(MediaStatus.NonStandard, "validation.rom.nonStandard");
    }

    public static MediaCheck CheckFloppy(string? path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            return new MediaCheck(MediaStatus.Missing, "validation.adf.missing");
        }

        if (!File.Exists(path))
        {
            return new MediaCheck(MediaStatus.Missing, "validation.adf.missing");
        }

        var length = new FileInfo(path).Length;
        var extensionOk = path.EndsWith(".adf", StringComparison.OrdinalIgnoreCase);

        foreach (var size in StandardAdfSizes)
        {
            if (length == size && extensionOk)
            {
                return new MediaCheck(MediaStatus.Recognised, "validation.adf.ok");
            }
        }

        return new MediaCheck(MediaStatus.NonStandard, "validation.adf.nonStandard");
    }
}
