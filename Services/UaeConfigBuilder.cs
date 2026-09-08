using System;
using System.Globalization;
using System.IO;
using System.Text;
using A500Launcher.Models;

namespace A500Launcher.Services;

public static class UaeConfigBuilder
{
    public static Func<bool> GamepadConnected { get; set; } = GamepadDetector.IsGamepadConnected;

    public static string BuildAndSave(AppSettings settings)
    {
        var builder = new StringBuilder();

        builder.AppendLine("config_description=Amiga 500");
        builder.AppendLine("config_hardware=true");
        builder.AppendLine("config_host=true");

        builder.AppendLine("cpu_type=68000");
        builder.AppendLine("cpu_model=68000");
        builder.AppendLine("fpu_model=0");
        builder.AppendLine("cpu_speed=real");
        builder.AppendLine("cpu_compatible=true");
        builder.AppendLine("cpu_24bit_addressing=true");

        builder.AppendLine("chipset=ocs");
        builder.AppendLine("chipset_compatible=A500");
        builder.AppendLine($"ntsc={(settings.NtscTiming ? "true" : "false")}");
        builder.AppendLine($"chipset_refreshrate={(settings.NtscTiming ? 60 : 50)}");

        builder.AppendLine("chipmem_size=1");
        builder.AppendLine($"bogomem_size={(settings.ExtraTrapdoorRam512k ? 1 : 0)}");
        builder.AppendLine("fastmem_size=0");
        builder.AppendLine("z3mem_size=0");
        builder.AppendLine("gfxcard_size=0");

        if (!string.IsNullOrWhiteSpace(settings.KickstartRomPath))
        {
            builder.AppendLine($"kickstart_rom_file={EscapePath(settings.KickstartRomPath)}");
        }

        builder.AppendLine("floppy0type=0");
        builder.AppendLine("floppy1type=0");
        builder.AppendLine("floppy2type=-1");
        builder.AppendLine("floppy3type=-1");
        builder.AppendLine("nr_floppies=2");

        if (!string.IsNullOrWhiteSpace(settings.Floppy0Path))
        {
            builder.AppendLine($"floppy0={EscapePath(settings.Floppy0Path)}");
        }

        if (!string.IsNullOrWhiteSpace(settings.Floppy1Path))
        {
            builder.AppendLine($"floppy1={EscapePath(settings.Floppy1Path)}");
        }

        var slot = 0;
        foreach (var disk in settings.SwapDisks)
        {
            if (!string.IsNullOrWhiteSpace(disk))
            {
                builder.AppendLine($"diskimage{slot}={EscapePath(disk)}");
                slot++;
            }
        }

        builder.AppendLine("hardfile2=");
        builder.AppendLine("uaehf0=");
        builder.AppendLine("cd32cd=false");
        builder.AppendLine("scsi=false");

        builder.AppendLine($"gfx_fullscreen_amiga={(settings.Fullscreen ? "fullscreen" : "window")}");
        builder.AppendLine("gfx_width=720");
        builder.AppendLine("gfx_height=568");
        builder.AppendLine("gfx_correct_aspect=true");

        builder.AppendLine("sound_output=speaker");
        builder.AppendLine("sound_frequency=44100");
        builder.AppendLine("sound_channels=stereo");
        builder.AppendLine("sound_stereo_separation=7");
        builder.AppendLine($"sound_volume={ClampVolume(settings.MasterVolume).ToString(CultureInfo.InvariantCulture)}");

        builder.AppendLine("joyport0=mouse");
        builder.AppendLine($"joyport1={Port1Value(settings.Port1Device)}");
        builder.AppendLine("joyport1mode=djoy");

        AppendFilter(builder, settings.Filter);

        var tempDir = Path.Combine(Path.GetTempPath(), "A500Launcher");
        Directory.CreateDirectory(tempDir);
        CleanPreviousSessions(tempDir);

        var configPath = Path.Combine(tempDir, "a500_session.uae");
        File.WriteAllText(configPath, builder.ToString(), Encoding.UTF8);

        return configPath;
    }

    private static void CleanPreviousSessions(string tempDir)
    {
        foreach (var file in Directory.EnumerateFiles(tempDir, "*.uae"))
        {
            try
            {
                File.Delete(file);
            }
            catch (IOException)
            {
            }
        }
    }

    private static string EscapePath(string path) => path.Replace("\\", "\\\\");

    private static int ClampVolume(int percent)
    {
        var clamped = Math.Max(0, Math.Min(100, percent));
        return 100 - clamped;
    }

    private static string Port1Value(InputPort1 device) => device switch
    {
        InputPort1.Mouse => "mouse",
        InputPort1.Joystick => GamepadConnected() ? "joy0" : "joy1",
        _ => "none",
    };

    private static void AppendFilter(StringBuilder builder, ScreenFilter filter)
    {
        switch (filter)
        {
            case ScreenFilter.CrtLight:
                builder.AppendLine("gfx_filter=null");
                builder.AppendLine("gfx_filter_scanlines=32");
                builder.AppendLine("gfx_filter_scanlineratio=1");
                break;
            case ScreenFilter.CrtHeavy:
                builder.AppendLine("gfx_filter=null");
                builder.AppendLine("gfx_filter_scanlines=128");
                builder.AppendLine("gfx_filter_scanlineratio=1");
                break;
            default:
                builder.AppendLine("gfx_filter=null");
                builder.AppendLine("gfx_filter_scanlines=0");
                break;
        }
    }
}
