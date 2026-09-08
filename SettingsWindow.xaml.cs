using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using A500Launcher.I18n;
using A500Launcher.Models;

namespace A500Launcher;

public partial class SettingsWindow : Window
{
    public SettingsWindow(AppSettings current)
    {
        InitializeComponent();

        if ((Keyboard.Modifiers & (ModifierKeys.Control | ModifierKeys.Shift)) == (ModifierKeys.Control | ModifierKeys.Shift)
            || current.NtscTiming)
        {
            NtscCheck.Visibility = Visibility.Visible;
        }

        NtscCheck.IsChecked = current.NtscTiming;

        WinUaePathBox.Text = current.WinUaeExePath;
        KickstartPathBox.Text = current.KickstartRomPath;
        AdfFolderBox.Text = current.DefaultAdfFolder;
        TrapdoorRamCheck.IsChecked = current.ExtraTrapdoorRam512k;
        FullscreenCheck.IsChecked = current.Fullscreen;
        BootSplashCheck.IsChecked = current.ShowBootSplash;
        FloppySoundCheck.IsChecked = current.PlayFloppySound;

        LanguageCombo.ItemsSource = LanguageCatalog.Available();
        LanguageCombo.SelectedItem = LanguageCombo.Items
            .Cast<LanguageOption>()
            .FirstOrDefault(option => option.Code == (current.LanguageCode ?? Strings.Provider.Current));

        Port1Combo.SelectedIndex = (int)current.Port1Device;
        FilterCombo.SelectedIndex = (int)current.Filter;
        VolumeSlider.Value = current.MasterVolume;
    }

    public void CopyInto(AppSettings target)
    {
        target.WinUaeExePath = WinUaePathBox.Text;
        target.KickstartRomPath = KickstartPathBox.Text;
        target.DefaultAdfFolder = AdfFolderBox.Text;
        target.ExtraTrapdoorRam512k = TrapdoorRamCheck.IsChecked == true;
        target.Fullscreen = FullscreenCheck.IsChecked == true;
        target.ShowBootSplash = BootSplashCheck.IsChecked == true;
        target.PlayFloppySound = FloppySoundCheck.IsChecked == true;
        target.LanguageCode = (LanguageCombo.SelectedItem as LanguageOption)?.Code;
        target.Port1Device = (InputPort1)Port1Combo.SelectedIndex;
        target.Filter = (ScreenFilter)FilterCombo.SelectedIndex;
        target.MasterVolume = (int)VolumeSlider.Value;
        target.NtscTiming = NtscCheck.IsChecked == true;
    }

    private void OnVolumeChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        if (VolumeValue is not null)
        {
            VolumeValue.Text = ((int)e.NewValue).ToString(CultureInfo.CurrentCulture) + "%";
        }
    }

    private void OnBrowseWinUae_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new OpenFileDialog
        {
            Title = Strings.T("settings.browse.winuae.title"),
            Filter = Strings.T("settings.browse.winuae.filter"),
        };

        if (dialog.ShowDialog() == true)
        {
            WinUaePathBox.Text = dialog.FileName;
        }
    }

    private void OnBrowseKickstart_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new OpenFileDialog
        {
            Title = Strings.T("settings.browse.kickstart.title"),
            Filter = Strings.T("picker.kickstart.filter"),
        };

        if (dialog.ShowDialog() == true)
        {
            KickstartPathBox.Text = dialog.FileName;
        }
    }

    private void OnBrowseAdfFolder_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new OpenFolderDialog
        {
            Title = Strings.T("settings.browse.adfFolder.title"),
        };

        if (dialog.ShowDialog() == true)
        {
            AdfFolderBox.Text = dialog.FolderName;
        }
    }

    private void OnSave_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = true;
        Close();
    }

    private void OnCancel_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}
