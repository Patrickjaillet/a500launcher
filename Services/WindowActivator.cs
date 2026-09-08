using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace A500Launcher.Services;

public static class WindowActivator
{
    [DllImport("user32.dll")]
    private static extern bool SetForegroundWindow(IntPtr handle);

    [DllImport("user32.dll")]
    private static extern bool ShowWindow(IntPtr handle, int command);

    private const int ShowRestore = 9;

    public static void BringExistingInstanceToFront()
    {
        var current = Process.GetCurrentProcess();

        foreach (var process in Process.GetProcessesByName(current.ProcessName))
        {
            if (process.Id == current.Id || process.MainWindowHandle == IntPtr.Zero)
            {
                continue;
            }

            ShowWindow(process.MainWindowHandle, ShowRestore);
            SetForegroundWindow(process.MainWindowHandle);
            return;
        }
    }
}
