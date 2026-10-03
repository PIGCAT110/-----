using System.IO;
using System.Windows;
using PowerManager.App.Infrastructure;
using PowerManager.App.Views;
using PowerManager.Core.Configuration;
using Application = System.Windows.Application;

namespace PowerManager.App;

public partial class App : Application
{
    private const string SingleInstanceMutexName = @"Local\PowerManager.App.SingleInstance.v1";

    private Mutex? _singleInstanceMutex;
    private SettingsStore? _settingsStore;
    private TrayIconManager? _trayIcon;
    private MainWindow? _mainWindow;
    private bool _trayResidentTipShown;

    /// <summary>应用正在整体退出。为 true 时主窗口不再拦截关闭。</summary>
    public new static App Current => (App)Application.Current;

    public AppSettings Settings { get; private set; } = new();

    public static bool IsShuttingDown { get; private set; }

    protected override void OnStartup(StartupEventArgs e)
    {
        _singleInstanceMutex = new Mutex(initiallyOwned: true, SingleInstanceMutexName, out var createdNew);
        if (!createdNew)
        {
            Shutdown();
            return;
        }

        base.OnStartup(e);
        ShutdownMode = ShutdownMode.OnExplicitShutdown;

        var settingsDirectory = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "PowerManager");
        _settingsStore = new SettingsStore(Path.Combine(settingsDirectory, "settings.json"));
        Settings = _settingsStore.Load();

        _trayIcon = new TrayIconManager(ShowMainWindow, OpenSettings, ExitApplication);

        _mainWindow = new MainWindow();
        MainWindow = _mainWindow;
        _mainWindow.Show();
    }

    public void ShowMainWindow()
    {
        if (_mainWindow is null)
        {
            return;
        }

        if (_mainWindow.WindowState == WindowState.Minimized)
        {
            _mainWindow.WindowState = WindowState.Normal;
        }

        _mainWindow.Show();
        _mainWindow.Activate();
    }

    public void OpenSettings()
    {
        var window = new SettingsWindow(Settings, _settingsStore!);
        if (_mainWindow?.IsVisible == true)
        {
            window.Owner = _mainWindow;
        }

        window.ShowDialog();
    }

    /// <summary>主窗口转入托盘时提示一次，避免用户误以为应用已退出。</summary>
    public void NotifyTrayResidentOnce()
    {
        if (_trayResidentTipShown || _trayIcon is null)
        {
            return;
        }

        _trayResidentTipShown = true;
        _trayIcon.ShowInfoTip("猪猫帮你关仍在运行", "已最小化到系统托盘，可右键托盘图标查看状态或退出应用。");
    }

    public void ExitApplication()
    {
        IsShuttingDown = true;
        Shutdown();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        _trayIcon?.Dispose();
        _singleInstanceMutex?.Dispose();
        base.OnExit(e);
    }
}
