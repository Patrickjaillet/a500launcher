namespace A500Launcher.I18n;

public static class Strings
{
    private static II18nProvider _provider = new JsonI18nProvider();

    public static II18nProvider Provider
    {
        get => _provider;
        set => _provider = value;
    }

    public static string T(string key) => _provider.T(key);

    public static string T(string key, params object[] args) => _provider.T(key, args);
}
