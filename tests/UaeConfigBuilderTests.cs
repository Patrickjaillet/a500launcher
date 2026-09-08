using System.IO;
using A500Launcher.Models;
using A500Launcher.Services;
using Xunit;

namespace A500Launcher.Tests;

public sealed class UaeConfigBuilderTests
{
    private static string Build(AppSettings settings) => File.ReadAllText(UaeConfigBuilder.BuildAndSave(settings));

    [Fact]
    public void LocksTheA500HardwareProfile()
    {
        var content = Build(new AppSettings());

        Assert.Contains("cpu_type=68000", content);
        Assert.Contains("cpu_model=68000", content);
        Assert.Contains("fpu_model=0", content);
        Assert.Contains("cpu_speed=real", content);
        Assert.Contains("cpu_24bit_addressing=true", content);
        Assert.Contains("chipset=ocs", content);
        Assert.Contains("chipset_compatible=A500", content);
        Assert.Contains("ntsc=false", content);
        Assert.Contains("chipset_refreshrate=50", content);
        Assert.Contains("chipmem_size=1", content);
        Assert.Contains("fastmem_size=0", content);
        Assert.Contains("z3mem_size=0", content);
        Assert.Contains("gfxcard_size=0", content);
        Assert.Contains("nr_floppies=2", content);
        Assert.Contains("floppy2type=-1", content);
        Assert.Contains("floppy3type=-1", content);
        Assert.Contains("cd32cd=false", content);
        Assert.Contains("scsi=false", content);
        Assert.Contains("joyport0=mouse", content);
    }

    [Fact]
    public void TrapdoorRamTogglesBogomem()
    {
        Assert.Contains("bogomem_size=1", Build(new AppSettings { ExtraTrapdoorRam512k = true }));
        Assert.Contains("bogomem_size=0", Build(new AppSettings { ExtraTrapdoorRam512k = false }));
    }

    [Fact]
    public void FullscreenTogglesGfxMode()
    {
        Assert.Contains("gfx_fullscreen_amiga=fullscreen", Build(new AppSettings { Fullscreen = true }));
        Assert.Contains("gfx_fullscreen_amiga=window", Build(new AppSettings { Fullscreen = false }));
    }

    [Fact]
    public void EscapesWindowsPaths()
    {
        var content = Build(new AppSettings
        {
            KickstartRomPath = @"C:\roms\kick13.rom",
            Floppy0Path = @"D:\adf\game.adf",
        });

        Assert.Contains(@"kickstart_rom_file=C:\\roms\\kick13.rom", content);
        Assert.Contains(@"floppy0=D:\\adf\\game.adf", content);
    }

    [Fact]
    public void Port1DeviceMapsToJoyport1()
    {
        Assert.Contains("joyport1=none", Build(new AppSettings { Port1Device = InputPort1.None }));
        Assert.Contains("joyport1=mouse", Build(new AppSettings { Port1Device = InputPort1.Mouse }));
        Assert.Contains("joyport1=joy1", Build(new AppSettings { Port1Device = InputPort1.Joystick }));
    }

    [Fact]
    public void MasterVolumeIsInvertedForWinUae()
    {
        Assert.Contains("sound_volume=0", Build(new AppSettings { MasterVolume = 100 }));
        Assert.Contains("sound_volume=100", Build(new AppSettings { MasterVolume = 0 }));
        Assert.Contains("sound_volume=75", Build(new AppSettings { MasterVolume = 25 }));
    }

    [Fact]
    public void NtscTimingSwitchesRefreshRate()
    {
        var pal = Build(new AppSettings { NtscTiming = false });
        Assert.Contains("ntsc=false", pal);
        Assert.Contains("chipset_refreshrate=50", pal);

        var ntsc = Build(new AppSettings { NtscTiming = true });
        Assert.Contains("ntsc=true", ntsc);
        Assert.Contains("chipset_refreshrate=60", ntsc);
    }

    [Fact]
    public void ScreenFilterEmitsScanlines()
    {
        Assert.Contains("gfx_filter_scanlines=0", Build(new AppSettings { Filter = ScreenFilter.Crisp }));
        Assert.Contains("gfx_filter_scanlines=32", Build(new AppSettings { Filter = ScreenFilter.CrtLight }));
        Assert.Contains("gfx_filter_scanlines=128", Build(new AppSettings { Filter = ScreenFilter.CrtHeavy }));
    }

    [Fact]
    public void OmitsEmptyMediaLines()
    {
        var content = Build(new AppSettings());

        Assert.DoesNotContain("\nfloppy0=", content);
        Assert.DoesNotContain("\nfloppy1=", content);
        Assert.DoesNotContain("kickstart_rom_file=", content);
    }
}
