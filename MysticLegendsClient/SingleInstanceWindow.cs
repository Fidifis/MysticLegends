namespace MysticLegendsClient;

public interface ISingleInstanceWindow
{
    public void ShowWindow();
    public void Close();
    public event EventHandler Closed;
}

public sealed class SingleInstanceWindow<T> : IDisposable where T : ISingleInstanceWindow, new()
{
    private ISingleInstanceWindow? instance;
    public ISingleInstanceWindow Instance
    {
        get
        {
            instance ??= new T();
            instance.Closed += (object? s, EventArgs e) => { instance = null; };
            return instance;
        }
    }

    public void Dispose()
    {
        instance?.Close();
    }
}
