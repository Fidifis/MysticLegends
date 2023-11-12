using System.Windows;

namespace MysticLegendsClient;

internal class ErrorCatcher
{
    public static void Try(Action action)
    {
        try
        {
            action();
        }
        catch (Exception e)
        {
            MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    public async static Task TryAsync(Func<Task> action)
    {
        try
        {
            await action();
        }
        catch (Exception e)
        {
            MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    public async static Task<T?> TryAsync<T>(Func<Task<T>> action)
    {
        try
        {
            return await action();
        }
        catch (Exception e)
        {
            MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        return default;
    }
}
