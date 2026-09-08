using System;
using System.IO;
using A500Launcher.Services;
using Xunit;

namespace A500Launcher.Tests;

public sealed class MediaValidationTests : IDisposable
{
    private readonly string _dir = Path.Combine(Path.GetTempPath(), "a500-mv-" + Guid.NewGuid().ToString("N"));

    public MediaValidationTests() => Directory.CreateDirectory(_dir);

    public void Dispose() => Directory.Delete(_dir, recursive: true);

    private string Make(string name, long size)
    {
        var path = Path.Combine(_dir, name);
        using var stream = new FileStream(path, FileMode.Create);
        stream.SetLength(size);
        return path;
    }

    [Fact]
    public void MissingRomIsReported()
    {
        Assert.Equal(MediaStatus.Missing, MediaValidation.CheckKickstart(null).Status);
        Assert.Equal(MediaStatus.Missing, MediaValidation.CheckKickstart(@"C:\nope\kick.rom").Status);
    }

    [Fact]
    public void PowerOfTwoRomInRangeIsRecognised()
    {
        Assert.Equal(MediaStatus.Recognised, MediaValidation.CheckKickstart(Make("kick13.rom", 262_144)).Status);
        Assert.Equal(MediaStatus.Recognised, MediaValidation.CheckKickstart(Make("kick20.rom", 524_288)).Status);
    }

    [Fact]
    public void OddSizedRomIsNonStandard()
    {
        Assert.Equal(MediaStatus.NonStandard, MediaValidation.CheckKickstart(Make("weird.rom", 300_000)).Status);
    }

    [Fact]
    public void StandardAdfIsRecognised()
    {
        Assert.Equal(MediaStatus.Recognised, MediaValidation.CheckFloppy(Make("game.adf", 901_120)).Status);
    }

    [Fact]
    public void WrongExtensionOrSizeIsNonStandard()
    {
        Assert.Equal(MediaStatus.NonStandard, MediaValidation.CheckFloppy(Make("game.img", 901_120)).Status);
        Assert.Equal(MediaStatus.NonStandard, MediaValidation.CheckFloppy(Make("game.adf", 123_456)).Status);
    }
}
