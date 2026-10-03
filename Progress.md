# Progress

> Windows 智能电源管理器项目的跨会话进度记录。
> 新会话开始时，AI 必须先阅读本文件，再执行任何开发任务。

## 使用规则

1. 每完成一个明确任务并通过验收后，先提交代码（commit），再更新本文件。
2. 每次更新必须记录：完成步骤、commit、当前架构或行为变更、验证结果、下一步。
3. 未完成、待验证或存在风险的内容不得标记为已完成。
4. 更新本文件后需再次提交，使代码与进度记录保持同步。
5. 新会话开始时，先读取本文件，并以“当前进度、最近变更、下一步”为依据继续工作；不要重复询问已有信息。

## 当前状态

- 项目阶段：阶段二 · 电源动作
- 当前步骤：3（TODO 任务 3：实现电源操作）
- 已完成步骤：2（TODO 任务 1 初始化项目、任务 2 基础界面和系统托盘）
- 最近 commit：93e7788
- 当前架构：.NET 8 / WPF，Core（领域规则/配置）、App（WPF + WinForms 托盘）、Core.Tests（xUnit）三项目分层
- 阻塞问题：无

## 进度记录

### Step 0：项目初始化

- 状态：✅ 已完成
- 完成时间：2026-10-03
- 目标：建立项目结构、开发规范和基础运行环境。
- 实现内容：
  - per-user 安装 .NET SDK 8.0.425 与 Windows Desktop Runtime 8.0.31（路径 `%LOCALAPPDATA%\Microsoft\dotnet`，已加入用户 PATH）。
  - 建立 `PowerManager.sln`：`src/PowerManager.Core`（net8.0，UI 无关领域层）、`src/PowerManager.App`（net8.0-windows WPF）、`tests/PowerManager.Core.Tests`（xUnit）。
  - App 配置 asInvoker 清单、PerMonitorV2 DPI、Win10 兼容 GUID、win-x64 框架依赖单文件发布。
  - 领域枚举 `PowerAction`（关机/重启/睡眠/休眠/注销/锁定）与首个测试。
- 验收结果：通过。`dotnet build` 0 警告 0 错误；`dotnet test` 1/1 通过；Debug 与 Release 发布版均成功启动，窗口标题"猪猫帮你关"。
- Commit：`ec7ee22`
- 当前架构变更：确立三层解决方案骨架；配置文件 `global.json`（rollForward latestMinor）。
- 风险或遗留问题：
  - 最终安装包（独立部署或引导安装运行时）在任务 12 确定；当前发布为框架依赖，目标机器需 .NET 8 Desktop Runtime。
  - 打包发布时需注意 CI/其他机器使用官方 SDK 安装方式。
- 下一步：建立基础界面、配置持久化与系统托盘常驻（TODO 任务 2）。

### Step 1：基础界面和系统托盘

- 状态：✅ 已完成
- 完成时间：2026-10-03
- 目标：主窗口可打开/关闭/恢复；关闭后后台常驻；托盘菜单支持查看状态、打开设置、退出应用。
- 实现内容：
  - Core：`Configuration/AppSettings`（MinimizeToTrayOnClose 等设置 POCO）、`SettingsStore`（JSON camelCase、缩进；文件缺失/目录缺失/JSON 损坏均回退默认值，Save 自动建目录）。
  - App 生命周期：`App.xaml.cs` 持有 Mutex 单实例（Local\PowerManager.App.SingleInstance.v1）、OnExplicitShutdown、设置文件 `%APPDATA%\PowerManager\settings.json`、ShowMainWindow/OpenSettings/ExitApplication/首次驻留气泡。
  - 托盘：`Infrastructure/TrayIconManager`（WinForms NotifyIcon；查看状态/设置…/分隔线/退出应用；双击=查看状态），动作以委托注入；`Assets/power.ico` 多尺寸图标。
  - 开机自启：`Infrastructure/WindowsStartup` 读写 HKCU `Software\Microsoft\Windows\CurrentVersion\Run`（值名 PowerManager）。
  - UI：`Views/MainWindow` 状态页（运行时长 DispatcherTimer、自启/关闭行为状态、设置/隐藏/退出三按钮，关闭时按设置最小化到托盘）；`Views/SettingsWindow` 模态设置（开机自启、关闭最小化到托盘，保存同时写注册表与 JSON）；App.xaml Win11 浅色风资源字典。
- 验收结果：通过。
  - `dotnet build`：0 警告 0 错误；`dotnet test`：5/5 通过（SettingsStore 缺失默认值、往返、建目录、损坏回退）。
  - UIA 端到端 16/16 通过：主窗口三按钮、设置对话框 2 复选框、勾选状态切换、取消/保存、JSON false/true 持久化、注册表未被误写、隐藏后进程存活、第二实例自动退出、退出按钮结束进程。
- Commit：`93e7788`
- 当前架构变更：
  - Core 新增 Configuration 目录；App 新增 Infrastructure（Win32/外壳相关）与 Views，原根目录 MainWindow 移入 Views。
  - App 启用 UseWindowsForms（仅供 NotifyIcon）；DPI 从 manifest 改为 csproj `ApplicationHighDpiMode=PerMonitorV2`（WinForms 启用后 manifest dpi 节会触发 WFAC010）。
- 风险或遗留问题：
  - 托盘右键弹出菜单本身未能 UIA 自动化：Win11 任务栏 XAML 外壳不响应注入式鼠标右键（mouse_event/SetFocus+Shift+F10/PostMessage WM_USER 回调均被外壳或前台规则拦截）。菜单项与事件绑定为标准 WinForms 写法，三个委托动作均已通过主窗口等效入口端到端验证；建议在真实桌面人工右键托盘图标抽查一次（含设置窗口从托盘打开）。
  - Win11 会在驻留气泡后把图标收入溢出区，属外壳默认行为；用户可将图标固定到任务栏。
  - 根目录 PRD.md / DESIGN.md / AGENTS.md 仍未纳入 git（任务 1 起即未跟踪），未做处理。
- 下一步：任务 3「实现电源操作」：定义 IPowerExecutor 与 Win32 实现（关机/重启/睡眠/休眠），统一执行前校验与明确错误；测试中用模拟实现隔离真实电源命令。

## 更新模板

完成任务并提交代码后，按以下格式追加记录：

```md
### Step N：任务名称

- 状态：✅ 已完成
- 完成时间：YYYY-MM-DD
- 目标：
- 实现内容：
- 验收标准：
- 验收结果：通过 / 未通过
- Commit：`commit-hash`
- 当前架构变更：
- 风险或遗留问题：无
- 下一步：
```

## Commit 规范

- `feat:` 新增功能
- `fix:` 修复问题
- `refactor:` 重构代码
- `test:` 增加或调整测试
- `docs:` 更新文档
- `chore:` 工程配置或维护

Commit 应小而完整，只包含一个清晰主题。