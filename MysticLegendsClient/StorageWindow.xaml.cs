using System.Windows;

namespace MysticLegendsClient;

/// <summary>
/// Interaction logic for StorageWindow.xaml
/// </summary>
public partial class StorageWindow : Window, ISingleInstanceWindow
{
    public StorageWindow(string cityName)
    {
        InitializeComponent();
    }

    public void ShowWindow()
    {
        SingleInstanceWindow.CommonShowWindowTasks(this);
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {

    }
}
