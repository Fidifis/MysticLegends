using MysticLegendsClient.Controls;
using MysticLegendsShared.Utilities;
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
        inventoryView.ItemDropEvent += ItemDrop;
    }

    public void ShowWindow()
    {
        SingleInstanceWindow.CommonShowWindowTasks(this);
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        _=RefreshStorageItems();
    }

    private async Task RefreshStorageItems()
    {
        await ErrorCatcher.TryAsync(async () =>
        {
            var inventory = await ApiCalls.CityCall.GetCityStorageAsync(cityName);
            inventoryView.FillData(inventory.InventoryItems, inventory.Capacity);
        });
    }

    private void ItemDrop(IItemView sender, ItemDropEventArgs args)
    {
        if (sender == inventoryView)
        {
            // Moving items within storage
            _=ErrorCatcher.TryAsync(async () =>
            {
                var inventory = await ApiCalls.CityCall.SwapStorageItemAsync(cityName, args.FromSlot.Item!.InvitemId, args.ToSlot.GridPosition);
                inventoryView.FillData(inventory.InventoryItems, inventory.Capacity);
            });
        }
        else if (args.IsHandover)
        {
            // Transfering item to character inventory
            _=ErrorCatcher.TryAsync(async () =>
            {
                var inventory = await ApiCalls.CityCall.RetreiveItemAsync(cityName, args.FromSlot.Item!.InvitemId, args.ToSlot.GridPosition);
                GameState.Current.GameEvents.CharacterInventoryUpdate(sender, new(inventory.InventoryItems.AsReadOnly()));
                await RefreshStorageItems();
            });
        }
        else
        {
            // Transfering item to storage
            _=ErrorCatcher.TryAsync(async () =>
            {
                var inventory = await ApiCalls.CityCall.StoreItemAsync(cityName, args.FromSlot.Item!.InvitemId, args.ToSlot.GridPosition);
                inventoryView.FillData(inventory.InventoryItems, inventory.Capacity);
                ApiCalls.CharacterCall.UpdateCharacter(this, GameState.Current.CharacterName);
            });
        }
    }
}
