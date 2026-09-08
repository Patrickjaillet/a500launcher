using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.RegularExpressions;
using A500Launcher.I18n;
using Xunit;

namespace A500Launcher.Tests;

public sealed class I18nTests
{
    private static string I18nDir => Path.Combine(TestContext.RepoRoot, "assets", "i18n");

    [Fact]
    public void FallbackProviderResolvesEveryKnownKey()
    {
        var provider = new JsonI18nProvider("en");

        foreach (var key in ReferenceKeys())
        {
            Assert.False(string.IsNullOrEmpty(provider.T(key)));
            Assert.NotEqual(key, provider.T(key));
        }
    }

    [Fact]
    public void EveryKeyReferencedInSourceExistsInEnglish()
    {
        var known = new HashSet<string>(ReferenceKeys());
        var pattern = new Regex("\"([a-z0-9]+(?:\\.[a-z0-9A-Z]+)+)\"");

        foreach (var file in Directory.EnumerateFiles(TestContext.RepoRoot, "*.cs", SearchOption.AllDirectories))
        {
            if (file.Contains($"{Path.DirectorySeparatorChar}tests{Path.DirectorySeparatorChar}"))
            {
                continue;
            }

            var text = File.ReadAllText(file);
            foreach (Match match in pattern.Matches(text))
            {
                var candidate = match.Groups[1].Value;
                if (candidate.StartsWith("dialog.") || candidate.StartsWith("menu.")
                    || candidate.StartsWith("desktop.") || candidate.StartsWith("boot.")
                    || candidate.StartsWith("picker.") || candidate.StartsWith("settings.")
                    || candidate.StartsWith("app.") || candidate.StartsWith("language.")
                    || candidate.StartsWith("error.") || candidate.StartsWith("validation.")
                    || candidate.StartsWith("library.") || candidate.StartsWith("editor."))
                {
                    Assert.Contains(candidate, known);
                }
            }
        }
    }

    [Fact]
    public void EveryTranslationFileHasExactlyTheReferenceKeys()
    {
        var reference = ReferenceMap();

        foreach (var file in Directory.EnumerateFiles(I18nDir, "*.json"))
        {
            if (Path.GetFileNameWithoutExtension(file) == "en")
            {
                continue;
            }

            var map = JsonSerializer.Deserialize<Dictionary<string, string>>(File.ReadAllText(file))!;

            var missing = reference.Keys.Where(key => !map.ContainsKey(key)).ToList();
            var unknown = map.Keys.Where(key => !reference.ContainsKey(key)).ToList();

            Assert.True(missing.Count == 0, $"{Path.GetFileName(file)} missing: {string.Join(", ", missing)}");
            Assert.True(unknown.Count == 0, $"{Path.GetFileName(file)} unknown: {string.Join(", ", unknown)}");
            Assert.False(string.IsNullOrWhiteSpace(map["language.name"]));
        }
    }

    [Fact]
    public void PlaceholderSlotsMatchTheReference()
    {
        var reference = ReferenceMap();
        var slot = new Regex("\\{(\\d+)\\}");

        string Slots(string value) => string.Join(
            ",",
            slot.Matches(value).Select(match => match.Groups[1].Value).Distinct().OrderBy(text => text));

        foreach (var file in Directory.EnumerateFiles(I18nDir, "*.json"))
        {
            if (Path.GetFileNameWithoutExtension(file) == "en")
            {
                continue;
            }

            var map = JsonSerializer.Deserialize<Dictionary<string, string>>(File.ReadAllText(file))!;
            foreach (var (key, value) in reference)
            {
                if (map.TryGetValue(key, out var translated))
                {
                    Assert.Equal(Slots(value), Slots(translated));
                }
            }
        }
    }

    private static IEnumerable<string> ReferenceKeys() => ReferenceMap().Keys;

    private static Dictionary<string, string> ReferenceMap()
    {
        return JsonSerializer.Deserialize<Dictionary<string, string>>(
            File.ReadAllText(Path.Combine(I18nDir, "en.json")))!;
    }
}
