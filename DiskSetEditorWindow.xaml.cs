using System.IO;
using System.Windows;
using Microsoft.Win32;
using A500Launcher.I18n;
using A500Launcher.Models;

namespace A500Launcher;

public partial class DiskSetEditorWindow : Window
{
    public DiskSetEditorWindow(DiskSet set)
    {
        InitializeComponent();
        Set = set;

        TitleBox.Text = set.Title;
        PublisherBox.Text = set.Publisher;
        YearBox.Text = set.Year;
        NotesBox.Text = set.Notes;
        Df0Box.Text = set.Floppy0Path;
        Df1Box.Text = set.Floppy1Path;
        TrapdoorCheck.IsChecked = set.ExtraTrapdoorRam512k;
        FullscreenCheck.IsChecked = set.Fullscreen;
    }

    public DiskSet Set { get; }

    private void OnBrowseDf0(object sender, RoutedEventArgs e) => Browse(Df0Box);

    private void OnBrowseDf1(object sender, RoutedEventArgs e) => Browse(Df1Box);

    private void Browse(System.Windows.Controls.TextBox target)
    {
        var dialog = new OpenFileDialog
        {
            Title = Strings.T("picker.df0.title"),
            Filter = Strings.T("picker.floppy.filter"),
            InitialDirectory = Directory.Exists(Path.GetDirectoryName(target.Text))
                ? Path.GetDirectoryName(target.Text)
                : string.Empty,
        };

        if (dialog.ShowDialog() == true)
        {
            target.Text = dialog.FileName;
        }
    }

    private void OnSave(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(TitleBox.Text))
        {
            MessageBox.Show(
                Strings.T("editor.titleRequired"),
                Strings.T("editor.title"),
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
            return;
        }

        Set.Title = TitleBox.Text.Trim();
        Set.Publisher = PublisherBox.Text.Trim();
        Set.Year = YearBox.Text.Trim();
        Set.Notes = NotesBox.Text.Trim();
        Set.Floppy0Path = Df0Box.Text;
        Set.Floppy1Path = Df1Box.Text;
        Set.ExtraTrapdoorRam512k = TrapdoorCheck.IsChecked == true;
        Set.Fullscreen = FullscreenCheck.IsChecked == true;

        DialogResult = true;
        Close();
    }

    private void OnCancel(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}
