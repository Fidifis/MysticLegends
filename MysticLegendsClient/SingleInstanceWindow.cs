﻿using System.Diagnostics;

namespace MysticLegendsClient;

public interface ISingleInstanceWindow
{
    public void ShowWindow();
    public void Close();
    public event EventHandler Closed;
}

public sealed class SingleInstanceWindow<T> : IDisposable where T : ISingleInstanceWindow, new()
{
    private bool disposed = false;
    private ISingleInstanceWindow? instance;
    public ISingleInstanceWindow Instance
    {
        get
        {
            ObjectDisposedException.ThrowIf(disposed, this);
            instance ??= new T();
            instance.Closed += (object? s, EventArgs e) => { instance = null; };
            return instance;
        }
    }

    public void Dispose()
    {
        instance?.Close();
        disposed = true;
    }
}

public sealed class SingleInstanceWindow : IDisposable
{
    public SingleInstanceWindow(Type instatiationType, params object?[]? args)
    {
        Debug.Assert(typeof(ISingleInstanceWindow).IsAssignableFrom(instatiationType), "the Type must implement ISingleInstanceWindow");
        this.instatiationType = instatiationType;
        this.args = args;
    }

    private bool disposed = false;
    private readonly Type instatiationType;
    private readonly object?[]? args;

    private ISingleInstanceWindow? instance;
    public ISingleInstanceWindow Instance
    {
        get
        {
            ObjectDisposedException.ThrowIf(disposed, this);
            instance ??= (ISingleInstanceWindow)(Activator.CreateInstance(instatiationType, args) ?? throw new NullReferenceException());
            instance.Closed += (object? s, EventArgs e) => { instance = null; };
            return instance;
        }
    }

    public void Dispose()
    {
        instance?.Close();
        disposed = true;
    }
}
