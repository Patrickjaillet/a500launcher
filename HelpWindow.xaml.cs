using System.Windows;

namespace A500Launcher;

public partial class HelpWindow : Window
{
    public HelpWindow()
    {
        InitializeComponent();
    }

    private void OnOk_Click(object sender, RoutedEventArgs e) => Close();
}
