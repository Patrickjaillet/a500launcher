using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using A500Launcher.I18n;
using A500Launcher.Models;
using A500Launcher.Services;

namespace A500Launcher;

public partial class LibraryWindow : Window
{
    private readonly AppSettings _settings;

    public LibraryWindow(AppSettings settings)
    {
        InitializeComponent();
        _settings = settings;
        Reload();
    }

    public DiskSet? LaunchRequested { get; private set; }

    private void Reload()
    {
        SetList.ItemsSource = DiskSetLibrary.LoadAll();
        UpdateButtons();
    }

    private DiskSet? Selected => SetList.SelectedItem as DiskSet;

    private void UpdateButtons()
    {
        var has = Selected is not null;
        LaunchButton.IsEnabled = has;
        EditButton.IsEnabled = has;
        DeleteButton.IsEnabled = has;
    }

    private void OnSelectionChanged(object sender, SelectionChangedEventArgs e) => UpdateButtons();

    private void OnList_DoubleClick(object sender, RoutedEventArgs e) => LaunchSelected();

    private void OnLaunch_Click(object sender, RoutedEventArgs e) => LaunchSelected();

    private void LaunchSelected()
    {
        if (Selected is null)
        {
            return;
        }

        LaunchRequested = Selected;
        DialogResult = true;
        Close();
    }

    private void OnEdit_Click(object sender, RoutedEventArgs e)
    {
        if (Selected is null)
        {
            return;
        }

        var editor = new DiskSetEditorWindow(Clone(Selected)) { Owner = this };
        if (editor.ShowDialog() == true)
        {
            DiskSetLibrary.Save(editor.Set);
            Reload();
        }
    }

    private void OnDelete_Click(object sender, RoutedEventArgs e)
    {
        if (Selected is null)
        {
            return;
        }

        var confirm = MessageBox.Show(
            Strings.T("library.delete.confirm", Selected.Title),
            Strings.T("library.delete"),
            MessageBoxButton.OKCancel,
            MessageBoxImage.Warning);

        if (confirm == MessageBoxResult.OK)
        {
            DiskSetLibrary.Delete(Selected.Id);
            Reload();
        }
    }

    private void OnSaveCurrent_Click(object sender, RoutedEventArgs e)
    {
        var editor = new DiskSetEditorWindow(DiskSetLibrary.FromSettings(_settings, string.Empty)) { Owner = this };
        if (editor.ShowDialog() == true)
        {
            DiskSetLibrary.Save(editor.Set);
            Reload();
        }
    }

    private void OnImport_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new OpenFolderDialog { Title = Strings.T("library.import.title") };
        if (dialog.ShowDialog() != true)
        {
            return;
        }

        var result = AdfImporter.Scan(dialog.FolderName, _settings.RecentFloppies);
        SettingsService.Save(_settings);

        MessageBox.Show(
            Strings.T("library.import.result", result.Added, result.Skipped),
            Strings.T("library.import"),
            MessageBoxButton.OK,
            MessageBoxImage.Information);
    }

    private void OnClose_Click(object sender, RoutedEventArgs e) => Close();

    private static DiskSet Clone(DiskSet source) => new()
    {
        Id = source.Id,
        Title = source.Title,
        Publisher = source.Publisher,
        Year = source.Year,
        Notes = source.Notes,
        Floppy0Path = source.Floppy0Path,
        Floppy1Path = source.Floppy1Path,
        ExtraTrapdoorRam512k = source.ExtraTrapdoorRam512k,
        Fullscreen = source.Fullscreen,
    };
}
