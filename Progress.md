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

- 项目阶段：阶段一 · 项目基础
- 当前步骤：2（TODO 任务 2：基础界面和系统托盘）
- 已完成步骤：1（TODO 任务 1：初始化项目）
- 最近 commit：ec7ee22
- 当前架构：.NET 8 / WPF，Core（领域规则）、App（WPF）、Core.Tests（xUnit）三项目分层
- 阻塞问题：无（winget 安装 SDK 受 UAC 限制，已改用官方 dotnet-install.ps1 per-user 安装）

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