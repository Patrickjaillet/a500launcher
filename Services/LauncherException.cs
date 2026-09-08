using System;

namespace A500Launcher.Services;

public sealed class LauncherException : Exception
{
    public LauncherException(string messageKey)
        : base(messageKey)
    {
        MessageKey = messageKey;
    }

    public LauncherException(string messageKey, Exception inner)
        : base(messageKey, inner)
    {
        MessageKey = messageKey;
    }

    public string MessageKey { get; }
}
