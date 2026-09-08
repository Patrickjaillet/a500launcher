using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Win32;
using A500Launcher.I18n;
using A500Launcher.Models;
using A500Launcher.Services;

namespace A500Launcher;

public partial class MainWindow : Window
{
    private readonly AppSettings _settings;

    public MainWindow()
    {
        InitializeComponent();
        _settings = App.Settings;
        StatusBarText.Text = Strings.T("boot.status.default");
        RefreshStatusLabels();
    }

    private void RefreshStatusLabels()
    {
        KickstartStatusText.Text = string.IsNullOrWhiteSpace(_settings.KickstartRomPath)
            ? Strings.T("desktop.kickstart.unset")
            : Path.GetFileName(_settings.KickstartRomPath);

        SetDriveLabel(Floppy0StatusText, Drive0EjectButton, _settings.Floppy0Path);
        SetDriveLabel(Floppy1StatusText, Drive1EjectButton, _settings.Floppy1Path);

        BuildRecentMenu(Drive0RecentMenu, drive0: true);
        BuildRecentMenu(Drive1RecentMenu, drive0: false);
    }

    private static void SetDriveLabel(TextBlock label, Button ejectButton, string path)
    {
        var empty = string.IsNullOrWhiteSpace(path);
        label.Text = empty ? Strings.T("desktop.floppy.empty") : Path.GetFileName(path);
        ejectButton.Visibility = empty ? Visibility.Collapsed : Visibility.Visible;
    }

    private void BuildRecentMenu(MenuItem parent, bool drive0)
    {
        parent.Items.Clear();
        var recent = RecentFiles.Existing(_settings.RecentFloppies);

        if (recent.Count == 0)
        {
            parent.Items.Add(new MenuItem { Header = Strings.T("menu.recent.empty"), IsEnabled = false });
            return;
        }

        foreach (var path in recent)
        {
            var item = new MenuItem { Header = Path.GetFileName(path), ToolTip = path };
            var captured = path;
            item.Click += (_, _) => InsertFloppy(drive0, captured);
            parent.Items.Add(item);
        }
    }

    private void InsertFloppy(bool drive0, string path)
    {
        if (drive0)
        {
            _settings.Floppy0Path = path;
        }
        else
        {
            _settings.Floppy1Path = path;
        }

        var folder = Path.GetDirectoryName(path);
        if (!string.IsNullOrEmpty(folder))
        {
            _settings.DefaultAdfFolder = folder;
        }

        RecentFiles.Add(_settings.RecentFloppies, path);
        SettingsService.Save(_settings);
        RefreshStatusLabels();
        Sounds.PlayFloppyInsert(_settings.PlayFloppySound);
    }

    private void EjectFloppy(bool drive0)
    {
        if (drive0)
        {
            _settings.Floppy0Path = string.Empty;
        }
        else
        {
            _settings.Floppy1Path = string.Empty;
        }

        SettingsService.Save(_settings);
        RefreshStatusLabels();
    }

    private void SetKickstart(string path)
    {
        _settings.KickstartRomPath = path;
        SettingsService.Save(_settings);
        RefreshStatusLabels();
    }

    private void OnWindowKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Escape)
        {
            Close();
        }
    }

    private void OnPickKickstart_Click(object sender, MouseButtonEventArgs e)
    {
        var dialog = new OpenFileDialog
        {
            Title = Strings.T("picker.kickstart.title"),
            Filter = Strings.T("picker.kickstart.filter"),
        };

        if (dialog.ShowDialog() == true)
        {
            SetKickstart(dialog.FileName);
        }
    }

    private void OnPickFloppy0_Click(object sender, MouseButtonEventArgs e) => PickFloppy(drive0: true);

    private void OnPickFloppy1_Click(object sender, MouseButtonEventArgs e) => PickFloppy(drive0: false);

    private void OnDrive0Insert(object sender, RoutedEventArgs e) => PickFloppy(drive0: true);

    private void OnDrive1Insert(object sender, RoutedEventArgs e) => PickFloppy(drive0: false);

    private void OnDrive0Eject(object sender, RoutedEventArgs e) => EjectFloppy(drive0: true);

    private void OnDrive1Eject(object sender, RoutedEventArgs e) => EjectFloppy(drive0: false);

    private void PickFloppy(bool drive0)
    {
        var dialog = new OpenFileDialog
        {
            Title = Strings.T(drive0 ? "picker.df0.title" : "picker.df1.title"),
            Filter = Strings.T("picker.floppy.filter"),
            InitialDirectory = Directory.Exists(_settings.DefaultAdfFolder) ? _settings.DefaultAdfFolder : string.Empty,
        };

        if (dialog.ShowDialog() == true)
        {
            InsertFloppy(drive0, dialog.FileName);
        }
    }

    private void OnOpenAdfFolder(object sender, RoutedEventArgs e)
    {
        if (Directory.Exists(_settings.DefaultAdfFolder))
        {
            Process.Start(new ProcessStartInfo(_settings.DefaultAdfFolder) { UseShellExecute = true });
        }
    }

    private void OnDragOverAny(object sender, DragEventArgs e) => AllowIfAnyFile(e);

    private void OnDragOverRom(object sender, DragEventArgs e) => AllowIfExtension(e, ".rom", ".bin");

    private void OnDragOverAdf(object sender, DragEventArgs e) => AllowIfExtension(e, ".adf");

    private static void AllowIfAnyFile(DragEventArgs e)
    {
        e.Effects = e.Data.GetDataPresent(DataFormats.FileDrop) ? DragDropEffects.Copy : DragDropEffects.None;
        e.Handled = true;
    }

    private static void AllowIfExtension(DragEventArgs e, params string[] extensions)
    {
        var path = SingleDroppedFile(e);
        var ok = path is not null && extensions.Any(ext => path.EndsWith(ext, StringComparison.OrdinalIgnoreCase));
        e.Effects = ok ? DragDropEffects.Copy : DragDropEffects.None;
        e.Handled = true;
    }

    private static string? SingleDroppedFile(DragEventArgs e)
    {
        if (!e.Data.GetDataPresent(DataFormats.FileDrop))
        {
            return null;
        }

        var files = (string[])e.Data.GetData(DataFormats.FileDrop);
        return files.Length == 1 ? files[0] : null;
    }

    private void OnWindowDrop(object sender, DragEventArgs e)
    {
        var path = SingleDroppedFile(e);
        if (path is null)
        {
            return;
        }

        if (path.EndsWith(".adf", StringComparison.OrdinalIgnoreCase))
        {
            InsertFloppy(drive0: true, path);
        }
        else if (path.EndsWith(".rom", StringComparison.OrdinalIgnoreCase)
              || path.EndsWith(".bin", StringComparison.OrdinalIgnoreCase))
        {
            SetKickstart(path);
        }
    }

    private void OnDrive0Drop(object sender, DragEventArgs e) => DropFloppy(e, drive0: true);

    private void OnDrive1Drop(object sender, DragEventArgs e) => DropFloppy(e, drive0: false);

    private void DropFloppy(DragEventArgs e, bool drive0)
    {
        var path = SingleDroppedFile(e);
        if (path is not null && path.EndsWith(".adf", StringComparison.OrdinalIgnoreCase))
        {
            InsertFloppy(drive0, path);
        }

        e.Handled = true;
    }

    private void OnKickstartDrop(object sender, DragEventArgs e)
    {
        var path = SingleDroppedFile(e);
        if (path is not null
            && (path.EndsWith(".rom", StringComparison.OrdinalIgnoreCase)
             || path.EndsWith(".bin", StringComparison.OrdinalIgnoreCase)))
        {
            SetKickstart(path);
        }

        e.Handled = true;
    }

    private void OnMenuFloppies_Click(object sender, RoutedEventArgs e)
    {
        MessageBox.Show(
            Strings.T("dialog.floppies.body"),
            Strings.T("dialog.floppies.title"),
            MessageBoxButton.OK,
            MessageBoxImage.Information);
    }

    private void OnMenuSettings_Click(object sender, RoutedEventArgs e)
    {
        var settingsWindow = new SettingsWindow(_settings) { Owner = this };
        if (settingsWindow.ShowDialog() == true)
        {
            settingsWindow.CopyInto(_settings);
            SettingsService.Save(_settings);
            RefreshStatusLabels();
        }
    }

    private void OnMenuAbout_Click(object sender, RoutedEventArgs e)
    {
        new AboutWindow { Owner = this }.ShowDialog();
    }

    private void OnMenuLibrary_Click(object sender, RoutedEventArgs e)
    {
        var library = new LibraryWindow(_settings) { Owner = this };
        var launch = library.ShowDialog() == true ? library.LaunchRequested : null;

        SettingsService.Save(_settings);
        RefreshStatusLabels();

        if (launch is not null)
        {
            DiskSetLibrary.ApplyTo(launch, _settings);
            SettingsService.Save(_settings);
            RefreshStatusLabels();
            OnBoot_Click(this, e);
        }
    }

    private void OnBoot_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(_settings.WinUaeExePath) || !File.Exists(_settings.WinUaeExePath))
        {
            MessageBox.Show(
                Strings.T("dialog.winuaeMissing.body"),
                Strings.T("dialog.winuaeMissing.title"),
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
            return;
        }

        if (string.IsNullOrWhiteSpace(_settings.KickstartRomPath) || !File.Exists(_settings.KickstartRomPath))
        {
            MessageBox.Show(
                Strings.T("dialog.romMissing.body"),
                Strings.T("dialog.romMissing.title"),
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
            return;
        }

        if (!WinUaeInfo.IsSupported(_settings.WinUaeExePath))
        {
            Log.Warn("Bundled or configured WinUAE is older than the supported minimum.");
            if (!ConfirmNonStandard("validation.winuae.old"))
            {
                return;
            }
        }

        var romCheck = MediaValidation.CheckKickstart(_settings.KickstartRomPath);
        if (romCheck.Status == MediaStatus.NonStandard && !ConfirmNonStandard(romCheck.DetailKey))
        {
            return;
        }

        foreach (var floppy in new[] { _settings.Floppy0Path, _settings.Floppy1Path })
        {
            if (string.IsNullOrWhiteSpace(floppy))
            {
                continue;
            }

            var floppyCheck = MediaValidation.CheckFloppy(floppy);
            if (floppyCheck.Status == MediaStatus.NonStandard && !ConfirmNonStandard(floppyCheck.DetailKey))
            {
                return;
            }
        }

        BootSplashWindow.RunIfEnabled(_settings.ShowBootSplash, this, StartEmulator);
    }

    private void StartEmulator()
    {
        try
        {
            StatusBarText.Text = Strings.T("boot.status.starting");
            var configPath = UaeConfigBuilder.BuildAndSave(_settings);
            EmulatorLauncher.Launch(_settings.WinUaeExePath, configPath);
            StatusBarText.Text = Strings.T("boot.status.default");
        }
        catch (LauncherException exception)
        {
            Log.Error("Startup failed.", exception);
            MessageBox.Show(
                Strings.T(exception.MessageKey),
                Strings.T("dialog.startError.title"),
                MessageBoxButton.OK,
                MessageBoxImage.Error);
            StatusBarText.Text = Strings.T("boot.status.error");
        }
    }

    private static bool ConfirmNonStandard(string detailKey)
    {
        return MessageBox.Show(
            Strings.T(detailKey),
            Strings.T("dialog.nonStandard.title"),
            MessageBoxButton.OKCancel,
            MessageBoxImage.Warning) == MessageBoxResult.OK;
    }
}
