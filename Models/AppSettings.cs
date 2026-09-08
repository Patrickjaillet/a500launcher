using System.Collections.Generic;

namespace A500Launcher.Models;

public sealed class AppSettings
{
    public List<string> RecentFloppies { get; set; } = new();

    public bool ShowBootSplash { get; set; } = true;

    public bool PlayFloppySound { get; set; } = true;

    public InputPort1 Port1Device { get; set; } = InputPort1.Mouse;

    public ScreenFilter Filter { get; set; } = ScreenFilter.Crisp;

    public int MasterVolume { get; set; } = 100;

    public string WinUaeExePath { get; set; } = string.Empty;

    public string KickstartRomPath { get; set; } = string.Empty;

    public string Floppy0Path { get; set; } = string.Empty;

    public string Floppy1Path { get; set; } = string.Empty;

    public string DefaultAdfFolder { get; set; } = string.Empty;

    public string? LanguageCode { get; set; }

    public bool Fullscreen { get; set; }

    public bool ExtraTrapdoorRam512k { get; set; } = true;
}
