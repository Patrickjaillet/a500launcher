using System;
using System.Windows.Markup;

namespace A500Launcher.I18n;

[MarkupExtensionReturnType(typeof(string))]
public sealed class LocalizeExtension : MarkupExtension
{
    public LocalizeExtension()
    {
    }

    public LocalizeExtension(string key)
    {
        Key = key;
    }

    [ConstructorArgument("key")]
    public string Key { get; set; } = string.Empty;

    public override object ProvideValue(IServiceProvider serviceProvider) => Strings.T(Key);
}
