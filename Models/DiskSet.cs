namespace A500Launcher.Models;

public sealed class DiskSet
{
    public string Id { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string Publisher { get; set; } = string.Empty;

    public string Year { get; set; } = string.Empty;

    public string Notes { get; set; } = string.Empty;

    public string Floppy0Path { get; set; } = string.Empty;

    public string Floppy1Path { get; set; } = string.Empty;

    public bool ExtraTrapdoorRam512k { get; set; } = true;

    public bool Fullscreen { get; set; }
}
