using System;
using System.Threading;

namespace A500Launcher.Services;

public sealed class SingleInstance : IDisposable
{
    private const string MutexName = @"Local\A500Launcher.SingleInstance";

    private readonly Mutex _mutex;

    public SingleInstance()
    {
        _mutex = new Mutex(initiallyOwned: true, MutexName, out var created);
        IsFirstInstance = created;
    }

    public bool IsFirstInstance { get; }

    public void Dispose()
    {
        if (IsFirstInstance)
        {
            _mutex.ReleaseMutex();
        }

        _mutex.Dispose();
    }
}
