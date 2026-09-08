using System;
using System.IO;
using A500Launcher.Services;
using Xunit;

namespace A500Launcher.Tests;

public sealed class KickstartCatalogTests : IDisposable
{
    private readonly string _dir = Path.Combine(Path.GetTempPath(), "a500-kick-" + Guid.NewGuid().ToString("N"));

    public KickstartCatalogTests() => Directory.CreateDirectory(_dir);

    public void Dispose() => Directory.Delete(_dir, recursive: true);

    [Fact]
    public void UnknownRomIsNotIdentified()
    {
        var path = Path.Combine(_dir, "mystery.rom");
        File.WriteAllBytes(path, new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 });

        Assert.Null(KickstartCatalog.Identify(path));
        Assert.False(KickstartCatalog.IsA500Kickstart(path));
    }

    [Fact]
    public void MissingFileIsNotIdentified()
    {
        Assert.Null(KickstartCatalog.Identify(@"C:\nope\kick.rom"));
    }

    [Fact]
    public void KnownCrcTableCoversA500AndNonA500Roms()
    {
        Assert.True(KickstartCatalog.KnownCrcs.ContainsKey(0xA0DA88E6u));
        Assert.Contains("1.2", KickstartCatalog.KnownCrcs[0xA0DA88E6u]);
        Assert.Contains("1.3", KickstartCatalog.KnownCrcs[0xC4F0F55Fu]);
        Assert.Contains("2.04", KickstartCatalog.KnownCrcs[0x9ED783D0u]);
    }
}
