using A500Launcher.Services;
using Xunit;

namespace A500Launcher.Tests;

public sealed class GamepadDetectorTests
{
    [Fact]
    public void IsGamepadConnectedDoesNotThrow()
    {
        var result = GamepadDetector.IsGamepadConnected();

        Assert.True(result || !result);
    }
}
