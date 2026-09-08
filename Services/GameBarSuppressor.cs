using System;
using Microsoft.Win32;

namespace A500Launcher.Services;

public static class GameBarSuppressor
{
    public static void Apply(bool suppress)
    {
        try
        {
            using var gameBar = Registry.CurrentUser.CreateSubKey(@"Software\Microsoft\GameBar");
            gameBar?.SetValue("AutoGameModeEnabled", suppress ? 0 : 1, RegistryValueKind.DWord);
            gameBar?.SetValue("AllowAutoGameMode", suppress ? 0 : 1, RegistryValueKind.DWord);
            gameBar?.SetValue("ShowStartupPanel", suppress ? 0 : 1, RegistryValueKind.DWord);
            gameBar?.SetValue("GamePanelStartupTipIndex", 3, RegistryValueKind.DWord);
            gameBar?.SetValue("UseNexusForGameBarEnabled", suppress ? 0 : 1, RegistryValueKind.DWord);

            using var gameDvr = Registry.CurrentUser.CreateSubKey(@"System\GameConfigStore");
            gameDvr?.SetValue("GameDVR_Enabled", suppress ? 0 : 1, RegistryValueKind.DWord);
            gameDvr?.SetValue("GameDVR_FSEBehaviorMode", 2, RegistryValueKind.DWord);

            using var appCapture = Registry.CurrentUser.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\GameDVR");
            appCapture?.SetValue("AppCaptureEnabled", suppress ? 0 : 1, RegistryValueKind.DWord);

            Log.Info(suppress ? "Game Bar overlay suppressed." : "Game Bar overlay restored.");
        }
        catch (Exception exception) when (exception is UnauthorizedAccessException or System.Security.SecurityException)
        {
            Log.Warn("Could not change the Game Bar registry settings.");
        }
    }
}
