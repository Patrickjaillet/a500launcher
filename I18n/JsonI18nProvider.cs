using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace A500Launcher.I18n;

public sealed class JsonI18nProvider : II18nProvider
{
    private const string FallbackLanguage = "en";

    private readonly Dictionary<string, string> _strings;
    private readonly Dictionary<string, string> _fallback;

    public string Current { get; }

    public JsonI18nProvider(string? languageCode = null)
    {
        var dir = Path.Combine(AppContext.BaseDirectory, "assets", "i18n");
        _fallback = Load(Path.Combine(dir, FallbackLanguage + ".json"));

        var requested = languageCode ?? CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
        var path = Path.Combine(dir, requested + ".json");

        if (File.Exists(path))
        {
            _strings = Load(path);
            Current = requested;
        }
        else
        {
            _strings = _fallback;
            Current = FallbackLanguage;
        }
    }

    public string T(string key)
    {
        if (_strings.TryGetValue(key, out var value))
        {
            return value;
        }

        return _fallback.TryGetValue(key, out var fallbackValue) ? fallbackValue : key;
    }

    public string T(string key, params object[] args)
    {
        return string.Format(CultureInfo.CurrentCulture, T(key), args);
    }

    private static Dictionary<string, string> Load(string path)
    {
        if (!File.Exists(path))
        {
            return new Dictionary<string, string>(StringComparer.Ordinal);
        }

        using var stream = File.OpenRead(path);
        var parsed = JsonSerializer.Deserialize<Dictionary<string, string>>(stream);
        return parsed ?? new Dictionary<string, string>(StringComparer.Ordinal);
    }

    public IReadOnlyCollection<string> Keys => _fallback.Keys.ToList();
}
