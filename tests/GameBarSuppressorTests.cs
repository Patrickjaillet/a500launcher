using System;
using Microsoft.Win32;
using A500Launcher.Services;
using Xunit;

namespace A500Launcher.Tests;

public sealed class GameBarSuppressorTests : IDisposable
{
    private readonly object? _autoGameMode;
    private readonly object? _gameDvr;

    public GameBarSuppressorTests()
    {
        using var gameBar = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\GameBar");
        _autoGameMode = gameBar?.GetValue("AutoGameModeEnabled");
        using var store = Registry.CurrentUser.OpenSubKey(@"System\GameConfigStore");
        _gameDvr = store?.GetValue("GameDVR_Enabled");
    }

    public void Dispose()
    {
        Restore(@"Software\Microsoft\GameBar", "AutoGameModeEnabled", _autoGameMode);
        Restore(@"System\GameConfigStore", "GameDVR_Enabled", _gameDvr);
    }

    private static void Restore(string subKey, string name, object? value)
    {
        using var key = Registry.CurrentUser.OpenSubKey(subKey, writable: true);
        if (key is null)
        {
            return;
        }

        if (value is null)
        {
            key.DeleteValue(name, throwOnMissingValue: false);
        }
        else
        {
            key.SetValue(name, value);
        }
    }

    [Fact]
    public void ApplyWritesAndClearsTheSuppressionValues()
    {
        GameBarSuppressor.Apply(true);
        using (var gameBar = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\GameBar"))
        {
            Assert.Equal(0, Convert.ToInt32(gameBar!.GetValue("AutoGameModeEnabled")));
        }

        GameBarSuppressor.Apply(false);
        using (var gameBar = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\GameBar"))
        {
            Assert.Equal(1, Convert.ToInt32(gameBar!.GetValue("AutoGameModeEnabled")));
        }
    }
}
