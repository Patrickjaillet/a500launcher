using System;
using System.IO;
using System.Text.Json;
using A500Launcher.Models;

namespace A500Launcher.Services;

public static class SettingsService
{
    private static readonly string FolderPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "A500Launcher");

    private static readonly string FilePath = Path.Combine(FolderPath, "config.json");

    private static readonly string LegacyFolderPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "AmigaLauncher");

    private static readonly string LegacyFilePath = Path.Combine(LegacyFolderPath, "config.json");

    private static readonly JsonSerializerOptions WriteOptions = new() { WriteIndented = true };

    public static AppSettings Load()
    {
        MigrateLegacy();

        try
        {
            if (File.Exists(FilePath))
            {
                var json = File.ReadAllText(FilePath);
                var settings = JsonSerializer.Deserialize<AppSettings>(json);
                if (settings is not null)
                {
                    return settings;
                }
            }
        }
        catch (Exception exception) when (exception is IOException or JsonException or UnauthorizedAccessException)
        {
        }

        return new AppSettings();
    }

    public static void Save(AppSettings settings)
    {
        Directory.CreateDirectory(FolderPath);
        var json = JsonSerializer.Serialize(settings, WriteOptions);

        var tempPath = FilePath + ".tmp";
        File.WriteAllText(tempPath, json);
        File.Move(tempPath, FilePath, overwrite: true);
    }

    private static void MigrateLegacy()
    {
        try
        {
            if (File.Exists(FilePath) || !File.Exists(LegacyFilePath))
            {
                return;
            }

            Directory.CreateDirectory(FolderPath);
            File.Copy(LegacyFilePath, FilePath);
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
        {
        }
    }
}
