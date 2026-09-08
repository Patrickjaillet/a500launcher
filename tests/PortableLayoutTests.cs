using System.IO;
using A500Launcher.Models;
using A500Launcher.Services;
using Xunit;

namespace A500Launcher.Tests;

public sealed class PortableLayoutTests
{
    [Fact]
    public void ApplyDefaultsFillsAdfFolderWhenEmpty()
    {
        var settings = new AppSettings();

        var changed = PortableLayout.ApplyDefaults(settings);

        Assert.True(changed);
        Assert.Equal(PortableLayout.RomsDir, settings.DefaultAdfFolder);
    }

    [Fact]
    public void ApplyDefaultsKeepsUserDefinedExistingPaths()
    {
        var existing = Path.GetTempPath();
        var settings = new AppSettings { DefaultAdfFolder = existing };

        PortableLayout.ApplyDefaults(settings);

        Assert.Equal(existing, settings.DefaultAdfFolder);
    }
}
