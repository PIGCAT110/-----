namespace PowerManager.Core.Power;

/// <summary>
/// 应用可执行的电源动作，对应 PRD 3.x 中的动作集合。
/// </summary>
public enum PowerAction
{
    Shutdown,
    Restart,
    Sleep,
    Hibernate,
    Logoff,
    Lock,
}
