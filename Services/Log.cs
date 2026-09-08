using System;
using System.IO;
using System.Linq;

namespace A500Launcher.Services;

public static class Log
{
    private const int RetentionDays = 7;

    private static readonly object Gate = new();

    private static readonly string Directory = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "A500Launcher",
        "logs");

    public static void Info(string message) => Write("INFO", message);

    public static void Warn(string message) => Write("WARN", message);

    public static void Error(string message, Exception? exception = null)
    {
        Write("ERROR", exception is null ? message : $"{message} :: {exception.GetType().Name}: {exception.Message}");
    }

    private static void Write(string level, string message)
    {
        try
        {
            lock (Gate)
            {
                System.IO.Directory.CreateDirectory(Directory);
                Prune();
                var path = Path.Combine(Directory, $"a500launcher-{DateTime.Now:yyyyMMdd}.log");
                File.AppendAllText(path, $"{DateTime.Now:HH:mm:ss} {level} {message}{Environment.NewLine}");
            }
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
        {
        }
    }

    private static void Prune()
    {
        var cutoff = DateTime.Now.AddDays(-RetentionDays);
        foreach (var file in System.IO.Directory.EnumerateFiles(Directory, "a500launcher-*.log")
                     .Where(file => File.GetLastWriteTime(file) < cutoff))
        {
            try
            {
                File.Delete(file);
            }
            catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
            {
            }
        }
    }
}
