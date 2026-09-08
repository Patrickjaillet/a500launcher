using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace A500Launcher.I18n;

public sealed record LanguageOption(string Code, string Name);

public static class LanguageCatalog
{
    public static IReadOnlyList<LanguageOption> Available()
    {
        var dir = Path.Combine(AppContext.BaseDirectory, "assets", "i18n");
        if (!Directory.Exists(dir))
        {
            return new[] { new LanguageOption("en", "English") };
        }

        var options = new List<LanguageOption>();

        foreach (var file in Directory.EnumerateFiles(dir, "*.json").OrderBy(path => path, StringComparer.Ordinal))
        {
            var code = Path.GetFileNameWithoutExtension(file);
            var name = code;

            try
            {
                var map = JsonSerializer.Deserialize<Dictionary<string, string>>(File.ReadAllText(file));
                if (map is not null && map.TryGetValue("language.name", out var declared))
                {
                    name = declared;
                }
            }
            catch (Exception exception) when (exception is IOException or JsonException)
            {
            }

            options.Add(new LanguageOption(code, name));
        }

        return options.Count > 0 ? options : new[] { new LanguageOption("en", "English") };
    }
}
