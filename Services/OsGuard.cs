using System;

namespace A500Launcher.Services;

public static class OsGuard
{
    private const int Windows11MinimumBuild = 22000;

    public static bool IsSupported()
    {
        var os = Environment.OSVersion;
        return os.Platform == PlatformID.Win32NT
            && os.Version.Major >= 10
            && os.Version.Build >= Windows11MinimumBuild;
    }
}
