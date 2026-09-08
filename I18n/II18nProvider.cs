namespace A500Launcher.I18n;

public interface II18nProvider
{
    string Current { get; }

    string T(string key);

    string T(string key, params object[] args);
}
