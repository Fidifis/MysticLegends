using System.Windows;

namespace MysticLegendsClient;

public interface ISingleInstanceWindow
{
    public void ShowWindow();
    public event RoutedEventHandler Loaded;
    public event EventHandler Closed;
}

public class SingleInstanceWindow<T> where T : ISingleInstanceWindow, new()
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
}
