using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using A500Launcher.Models;

namespace A500Launcher.Services;

public static class DiskSetLibrary
{
    private static readonly string Directory = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "A500Launcher",
        "sets");

    private static readonly JsonSerializerOptions Options = new() { WriteIndented = true };

    public static IReadOnlyList<DiskSet> LoadAll()
    {
        if (!System.IO.Directory.Exists(Directory))
        {
            return Array.Empty<DiskSet>();
        }

        var result = new List<DiskSet>();

        foreach (var file in System.IO.Directory.EnumerateFiles(Directory, "*.json"))
        {
            try
            {
                var set = JsonSerializer.Deserialize<DiskSet>(File.ReadAllText(file));
                if (set is not null && !string.IsNullOrEmpty(set.Id))
                {
                    result.Add(set);
                }
            }
            catch (Exception exception) when (exception is IOException or JsonException)
            {
            }
        }

        return result.OrderBy(set => set.Title, StringComparer.CurrentCultureIgnoreCase).ToList();
    }

    public static void Save(DiskSet set)
    {
        if (string.IsNullOrEmpty(set.Id))
        {
            set.Id = Guid.NewGuid().ToString("N");
        }

        System.IO.Directory.CreateDirectory(Directory);
        var path = Path.Combine(Directory, set.Id + ".json");
        var tempPath = path + ".tmp";
        File.WriteAllText(tempPath, JsonSerializer.Serialize(set, Options));
        File.Move(tempPath, path, overwrite: true);
    }

    public static void Delete(string id)
    {
        var path = Path.Combine(Directory, id + ".json");
        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }

    public static DiskSet FromSettings(AppSettings settings, string title)
    {
        return new DiskSet
        {
            Title = title,
            Floppy0Path = settings.Floppy0Path,
            Floppy1Path = settings.Floppy1Path,
            ExtraTrapdoorRam512k = settings.ExtraTrapdoorRam512k,
            Fullscreen = settings.Fullscreen,
        };
    }

    public static void ApplyTo(DiskSet set, AppSettings settings)
    {
        settings.Floppy0Path = set.Floppy0Path;
        settings.Floppy1Path = set.Floppy1Path;
        settings.ExtraTrapdoorRam512k = set.ExtraTrapdoorRam512k;
        settings.Fullscreen = set.Fullscreen;
    }
}
