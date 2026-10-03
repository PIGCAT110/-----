using System.ComponentModel;
using System.Windows;
using System.Windows.Threading;
using PowerManager.App.Infrastructure;

namespace PowerManager.App.Views;

public partial class MainWindow : Window
{
    private readonly DispatcherTimer _uptimeTimer;
    private readonly DateTime _startedAt;

    public MainWindow()
    {
        InitializeComponent();

        _startedAt = DateTime.Now;
        _uptimeTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
        _uptimeTimer.Tick += (_, _) => UpdateUptime();
        _uptimeTimer.Start();

        IsVisibleChanged += (_, _) =>
        {
            if (IsVisible)
            {
                RefreshStatus();
            }
        };
    }

    private void RefreshStatus()
    {
        UpdateUptime();
        StartupText.Text = WindowsStartup.IsEnabled() ? "已开启" : "未开启";
        CloseBehaviorText.Text = App.Current.Settings.MinimizeToTrayOnClose
            ? "最小化到系统托盘，应用继续运行"
            : "直接退出应用";
        TrayHintText.Text = App.Current.Settings.MinimizeToTrayOnClose
            ? "关闭主窗口后，应用将继续在系统托盘运行。右键托盘图标可查看状态、打开设置或退出应用。"
            : "当前设置为关闭窗口即退出应用。可在设置中改为最小化到系统托盘。";
    }

    private void UpdateUptime()
    {
        var elapsed = DateTime.Now - _startedAt;
        UptimeText.Text = elapsed.ToString(@"hh\:mm\:ss");
    }

    protected override void OnClosing(CancelEventArgs e)
    {
        if (!App.IsShuttingDown && App.Current.Settings.MinimizeToTrayOnClose)
        {
            e.Cancel = true;
            Hide();
            App.Current.NotifyTrayResidentOnce();
        }

        base.OnClosing(e);
    }

    private void HideButton_Click(object sender, RoutedEventArgs e) => Hide();

    private void SettingsButton_Click(object sender, RoutedEventArgs e) => App.Current.OpenSettings();

    private void ExitButton_Click(object sender, RoutedEventArgs e) => App.Current.ExitApplication();
}
