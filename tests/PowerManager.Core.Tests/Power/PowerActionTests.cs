using PowerManager.Core.Power;
using Xunit;

namespace PowerManager.Core.Tests.Power;

public class PowerActionTests
{
    [Fact]
    public void Prd_action_set_is_complete()
    {
        var defined = Enum.GetValues<PowerAction>();

        Assert.Equal(6, defined.Length);
        Assert.All(
            new[]
            {
                PowerAction.Shutdown,
                PowerAction.Restart,
                PowerAction.Sleep,
                PowerAction.Hibernate,
                PowerAction.Logoff,
                PowerAction.Lock,
            },
            action => Assert.Contains(action, defined));
    }
}
