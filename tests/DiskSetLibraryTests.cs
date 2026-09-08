using System;
using System.Collections.Generic;
using System.IO;
using A500Launcher.Models;
using A500Launcher.Services;
using Xunit;

namespace A500Launcher.Tests;

public sealed class DiskSetLibraryTests
{
    [Fact]
    public void ExportThenImportRoundTripsAndAssignsANewId()
    {
        var path = Path.Combine(Path.GetTempPath(), "a500-" + Guid.NewGuid().ToString("N") + ".a500set");
        var original = new DiskSet
        {
            Id = "original",
            Title = "Monkey Island",
            Publisher = "Lucasfilm",
            Year = "1990",
            Floppy0Path = @"C:\adf\mi-disk1.adf",
            SwapDisks = new List<string> { @"C:\adf\mi-disk2.adf", @"C:\adf\mi-disk3.adf" },
        };

        try
        {
            DiskSetLibrary.Export(original, path);
            var imported = DiskSetLibrary.Import(path);

            Assert.NotNull(imported);
            Assert.Equal("Monkey Island", imported!.Title);
            Assert.Equal(2, imported.SwapDisks.Count);
            Assert.NotEqual("original", imported.Id);
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public void ImportOfGarbageReturnsNull()
    {
        var path = Path.Combine(Path.GetTempPath(), "a500-bad-" + Guid.NewGuid().ToString("N") + ".a500set");
        File.WriteAllText(path, "not json at all");

        try
        {
            Assert.Null(DiskSetLibrary.Import(path));
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public void FromSettingsCopiesFloppyAndHardwareFlags()
    {
        var settings = new AppSettings
        {
            Floppy0Path = @"C:\a.adf",
            Floppy1Path = @"C:\b.adf",
            ExtraTrapdoorRam512k = false,
            Fullscreen = true,
        };

        var set = DiskSetLibrary.FromSettings(settings, "Lemmings");

        Assert.Equal("Lemmings", set.Title);
        Assert.Equal(@"C:\a.adf", set.Floppy0Path);
        Assert.Equal(@"C:\b.adf", set.Floppy1Path);
        Assert.False(set.ExtraTrapdoorRam512k);
        Assert.True(set.Fullscreen);
    }

    [Fact]
    public void ApplyToOverwritesSettingsFloppyAndHardwareFlags()
    {
        var set = new DiskSet
        {
            Floppy0Path = @"D:\game.adf",
            Floppy1Path = string.Empty,
            ExtraTrapdoorRam512k = true,
            Fullscreen = false,
        };
        var settings = new AppSettings { Floppy0Path = @"C:\old.adf", Fullscreen = true };

        DiskSetLibrary.ApplyTo(set, settings);

        Assert.Equal(@"D:\game.adf", settings.Floppy0Path);
        Assert.Equal(string.Empty, settings.Floppy1Path);
        Assert.True(settings.ExtraTrapdoorRam512k);
        Assert.False(settings.Fullscreen);
    }
}
