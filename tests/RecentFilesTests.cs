using System.Collections.Generic;
using System.Linq;
using A500Launcher.Services;
using Xunit;

namespace A500Launcher.Tests;

public sealed class RecentFilesTests
{
    [Fact]
    public void AddPutsNewestFirst()
    {
        var recent = new List<string>();

        RecentFiles.Add(recent, @"C:\a.adf");
        RecentFiles.Add(recent, @"C:\b.adf");

        Assert.Equal(new[] { @"C:\b.adf", @"C:\a.adf" }, recent);
    }

    [Fact]
    public void AddMovesExistingEntryToFrontWithoutDuplicating()
    {
        var recent = new List<string> { @"C:\b.adf", @"C:\a.adf" };

        RecentFiles.Add(recent, @"c:\A.ADF");

        Assert.Equal(new[] { @"c:\A.ADF", @"C:\b.adf" }, recent);
    }

    [Fact]
    public void AddCapsAtTheLimit()
    {
        var recent = new List<string>();

        for (var i = 0; i < RecentFiles.Limit + 5; i++)
        {
            RecentFiles.Add(recent, $@"C:\disk{i}.adf");
        }

        Assert.Equal(RecentFiles.Limit, recent.Count);
        Assert.Equal($@"C:\disk{RecentFiles.Limit + 4}.adf", recent.First());
    }

    [Fact]
    public void AddIgnoresBlankPaths()
    {
        var recent = new List<string>();

        RecentFiles.Add(recent, "");
        RecentFiles.Add(recent, "   ");

        Assert.Empty(recent);
    }
}
