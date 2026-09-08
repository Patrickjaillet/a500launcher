using A500Launcher.Models;
using A500Launcher.Services;
using Xunit;

namespace A500Launcher.Tests;

public sealed class DiskSetLibraryTests
{
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
