using System;
using System.IO;
using A500Launcher.Services;
using Xunit;

namespace A500Launcher.Tests;

public sealed class WinUaeInfoTests
{
    [Fact]
    public void MissingExeReturnsNullAndIsTreatedAsSupported()
    {
        Assert.Null(WinUaeInfo.GetVersion(@"C:\nope\winuae64.exe"));
        Assert.True(WinUaeInfo.IsSupported(@"C:\nope\winuae64.exe"));
    }

    [Fact]
    public void BundledWinUaeIsRecognisedAndSupported()
    {
        var bundled = Path.Combine(TestContext.RepoRoot, "winuae", "winuae64.exe");
        if (!File.Exists(bundled))
        {
            return;
        }

        var version = WinUaeInfo.GetVersion(bundled);
        Assert.NotNull(version);
        Assert.True(version!.Major >= 5);
        Assert.True(WinUaeInfo.IsSupported(bundled));
    }
}
