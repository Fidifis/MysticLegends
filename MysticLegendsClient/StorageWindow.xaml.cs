using MysticLegendsClient.Controls;
using System.Windows;

namespace MysticLegendsClient;

/// <summary>
/// Interaction logic for StorageWindow.xaml
/// </summary>
public partial class StorageWindow : Window, ISingleInstanceWindow
{
    private readonly string cityName;

    public StorageWindow(string cityName)
    {
        InitializeComponent();
        this.cityName = cityName;
    }

    public void ShowWindow()
    {
        SingleInstanceWindow.CommonShowWindowTasks(this);
    }

    private async void Window_Loaded(object sender, RoutedEventArgs e)
    {
        await ErrorCatcher.TryAsync(async () =>
        {
            var inventory = await ApiCalls.CityCall.GetCityStorageAsync(cityName);
            inventoryView.FillData(inventory.InventoryItems, inventory.Capacity);
        });
    }
}
