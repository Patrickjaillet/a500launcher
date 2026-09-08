using System;
using System.Windows;
using System.Windows.Threading;
using A500Launcher.I18n;
using A500Launcher.Models;
using A500Launcher.Services;

namespace A500Launcher;

public partial class App : Application
{
    private SingleInstance? _singleInstance;

    public static AppSettings Settings { get; private set; } = new();

    private void OnStartup(object sender, StartupEventArgs e)
    {
        DispatcherUnhandledException += OnUnhandledException;

        Settings = SettingsService.Load();
        Strings.Provider = new JsonI18nProvider(Settings.LanguageCode);

        if (!OsGuard.IsSupported())
        {
            Log.Warn("Unsupported operating system.");
            MessageBox.Show(
                Strings.T("dialog.unsupportedOs.body"),
                Strings.T("dialog.unsupportedOs.title"),
                MessageBoxButton.OK,
                MessageBoxImage.Stop);
            Shutdown(1);
            return;
        }

        _singleInstance = new SingleInstance();
        if (!_singleInstance.IsFirstInstance)
        {
            Log.Info("Another instance is already running.");
            WindowActivator.BringExistingInstanceToFront();
            Shutdown(0);
            return;
        }

        if (PortableLayout.ApplyDefaults(Settings))
        {
            SettingsService.Save(Settings);
        }

        Log.Info("A500 Launcher started.");

        var window = new MainWindow();
        MainWindow = window;
        window.Show();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        _singleInstance?.Dispose();
        base.OnExit(e);
    }

    private void OnUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        Log.Error("Unhandled exception.", e.Exception);
        MessageBox.Show(
            Strings.T("dialog.startError.title"),
            Strings.T("dialog.startError.title"),
            MessageBoxButton.OK,
            MessageBoxImage.Error);
        e.Handled = true;
    }
}
