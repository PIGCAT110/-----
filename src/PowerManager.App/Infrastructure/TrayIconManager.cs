using System.Drawing;
using System.Windows;
using System.Windows.Forms;
using Application = System.Windows.Application;

namespace PowerManager.App.Infrastructure;

/// <summary>
/// 系统托盘图标与右键菜单封装。菜单动作通过构造函数注入，
/// 不直接操作窗口，保持生命周期逻辑集中在 App。
/// </summary>
public sealed class TrayIconManager : IDisposable
{
    private const string TooltipText = "猪猫帮你关";

    private readonly NotifyIcon _icon;
    private bool _disposed;

    public TrayIconManager(Action showStatus, Action showSettings, Action exitApplication)
    {
        ArgumentNullException.ThrowIfNull(showStatus);
        ArgumentNullException.ThrowIfNull(showSettings);
        ArgumentNullException.ThrowIfNull(exitApplication);

        _icon = new NotifyIcon
        {
            Icon = LoadEmbeddedIcon(),
            Text = TooltipText,
            Visible = true,
        };

        var menu = new ContextMenuStrip { Font = new Font("Microsoft YaHei UI", 9F) };
        var statusItem = new ToolStripMenuItem("查看状态");
        var settingsItem = new ToolStripMenuItem("设置…");
        var separator = new ToolStripSeparator();
        var exitItem = new ToolStripMenuItem("退出应用");

        statusItem.Click += (_, _) => showStatus();
        settingsItem.Click += (_, _) => showSettings();
        exitItem.Click += (_, _) => exitApplication();
        _icon.DoubleClick += (_, _) => showStatus();

        menu.Items.AddRange(new ToolStripItem[]
        {
            statusItem,
            settingsItem,
            separator,
            exitItem,
        });
        _icon.ContextMenuStrip = menu;
    }

    public void ShowInfoTip(string title, string message)
    {
        _icon.ShowBalloonTip(3000, title, message, ToolTipIcon.Info);
    }

    private static Icon LoadEmbeddedIcon()
    {
        var resourceUri = new Uri("pack://application:,,,/Assets/power.ico", UriKind.Absolute);
        using var stream = Application.GetResourceStream(resourceUri)!.Stream;
        return new Icon(stream);
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _icon.Visible = false;
        _icon.Dispose();
        _disposed = true;
    }
}
