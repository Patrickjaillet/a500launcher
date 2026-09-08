using System;
using System.Windows;
using System.Windows.Media.Animation;

namespace A500Launcher;

public partial class BootSplashWindow : Window
{
    private static readonly TimeSpan Duration = TimeSpan.FromMilliseconds(1500);

    public BootSplashWindow()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    public static void RunIfEnabled(bool enabled, Window owner, Action afterwards)
    {
        if (!enabled)
        {
            afterwards();
            return;
        }

        var splash = new BootSplashWindow { Owner = owner };
        splash.Closed += (_, _) => afterwards();
        splash.ShowDialog();
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        var blink = new DoubleAnimation(1.0, 0.15, TimeSpan.FromMilliseconds(250))
        {
            AutoReverse = true,
            RepeatBehavior = RepeatBehavior.Forever,
        };
        Led.BeginAnimation(OpacityProperty, blink);

        var timer = new System.Windows.Threading.DispatcherTimer { Interval = Duration };
        timer.Tick += (_, _) =>
        {
            timer.Stop();
            Close();
        };
        timer.Start();
    }
}
