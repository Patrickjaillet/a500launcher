using System;
using System.Diagnostics;
using System.IO;

namespace A500Launcher.Services;

public static class WinUaeInfo
{
    private const int MinimumMajor = 5;

    public static Version? GetVersion(string exePath)
    {
        if (string.IsNullOrWhiteSpace(exePath) || !File.Exists(exePath))
        {
            return null;
        }

        try
        {
            var info = FileVersionInfo.GetVersionInfo(exePath);
            if (info.FileMajorPart == 0 && info.FileMinorPart == 0)
            {
                return null;
            }

            return new Version(info.FileMajorPart, info.FileMinorPart, info.FileBuildPart);
        }
        catch (IOException)
        {
            return null;
        }
    }

    public static bool IsSupported(string exePath)
    {
        var version = GetVersion(exePath);
        return version is null || version.Major >= MinimumMajor;
    }
}
