using System;
using System.IO;
using System.Media;

namespace A500Launcher.Services;

public static class Sounds
{
    private static readonly Lazy<SoundPlayer?> FloppyInsert = new(() => Load("floppy-insert.wav"));

    public static void PlayFloppyInsert(bool enabled)
    {
        if (!enabled)
        {
            return;
        }

        try
        {
            FloppyInsert.Value?.Play();
        }
        catch (InvalidOperationException)
        {
        }
    }

    private static SoundPlayer? Load(string fileName)
    {
        var path = Path.Combine(AppContext.BaseDirectory, "assets", "sounds", fileName);
        if (!File.Exists(path))
        {
            return null;
        }

        var player = new SoundPlayer(path);
        player.LoadAsync();
        return player;
    }
}
