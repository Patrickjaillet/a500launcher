using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using A500Launcher.Services;
using Xunit;

namespace A500Launcher.Tests;

public sealed class AdfImporterTests : IDisposable
{
    private readonly string _dir = Path.Combine(Path.GetTempPath(), "a500-imp-" + Guid.NewGuid().ToString("N"));

    public AdfImporterTests() => Directory.CreateDirectory(_dir);

    public void Dispose() => Directory.Delete(_dir, recursive: true);

    private string Write(string name, string content)
    {
        var path = Path.Combine(_dir, name);
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        File.WriteAllText(path, content, Encoding.ASCII);
        return path;
    }

    [Fact]
    public void AddsNewImagesAndSkipsContentDuplicates()
    {
        Write("a.adf", "AAAA");
        Write(Path.Combine("sub", "a-copy.adf"), "AAAA");
        Write("b.adf", "BBBB");

        var known = new List<string>();
        var result = AdfImporter.Scan(_dir, known);

        Assert.Equal(2, result.Added);
        Assert.Equal(1, result.Skipped);
        Assert.Equal(2, known.Count);
    }

    [Fact]
    public void SkipsImagesAlreadyKnownByContent()
    {
        var existing = Write("known.adf", "SAME");
        Write("other.adf", "SAME");

        var known = new List<string> { existing };
        var result = AdfImporter.Scan(_dir, known);

        Assert.Equal(0, result.Added);
        Assert.Equal(2, result.Skipped);
        Assert.Single(known);
    }
}
