using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;

namespace A500Launcher.Services;

public static class EmulatorLauncher
{
    public static void Launch(string winUaeExePath, string uaeConfigPath)
    {
        if (string.IsNullOrWhiteSpace(winUaeExePath) || !File.Exists(winUaeExePath))
        {
            throw new LauncherException("dialog.winuaeMissing.body");
        }

        if (!File.Exists(uaeConfigPath))
        {
            throw new LauncherException("error.configNotWritten");
        }

        var startInfo = new ProcessStartInfo
        {
            FileName = winUaeExePath,
            Arguments = $"-f \"{uaeConfigPath}\"",
            UseShellExecute = true,
            WorkingDirectory = Path.GetDirectoryName(winUaeExePath) ?? Environment.CurrentDirectory,
        };

        try
        {
            var process = Process.Start(startInfo);
            Log.Info($"WinUAE started (pid {process?.Id.ToString() ?? "?"}).");
        }
        catch (Win32Exception exception)
        {
            Log.Error("WinUAE failed to start.", exception);
            throw new LauncherException("error.processRefused", exception);
        }
    }
}
