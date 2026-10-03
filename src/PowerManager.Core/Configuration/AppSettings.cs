namespace PowerManager.Core.Configuration;

/// <summary>
/// 应用通用设置，持久化到 settings.json。
/// 默认值遵循 PRD 安全原则：关闭主窗口不退出应用，转入托盘继续运行。
/// </summary>
public sealed class AppSettings
{
    /// <summary>关闭主窗口时最小化到系统托盘；为 false 时关闭窗口即退出应用。</summary>
    public bool MinimizeToTrayOnClose { get; set; } = true;
}
