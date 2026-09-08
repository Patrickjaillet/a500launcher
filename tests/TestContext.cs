using System;
using System.IO;

namespace A500Launcher.Tests;

internal static class TestContext
{
    public static string RepoRoot { get; } = Resolve();

    private static string Resolve()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir is not null && !File.Exists(Path.Combine(dir.FullName, "A500Launcher.sln")))
        {
            dir = dir.Parent;
        }

        return dir?.FullName ?? throw new DirectoryNotFoundException("A500Launcher.sln");
    }
}
