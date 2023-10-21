using MysticLegendsClasses;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Windows;

namespace MysticLegendsClient
{
    /// <summary>
    /// Interakční logika pro CharacterWindow.xaml
    /// </summary>
    public partial class CharacterWindow : Window
    {
        private static CharacterWindow? WindowInstance { get; set; } = null;
        public static void ShowWindow(Window? owner)
        {
            WindowInstance ??= new CharacterWindow() { Owner = owner };
            WindowInstance.Show();
            if (WindowInstance.WindowState == WindowState.Minimized) WindowInstance.WindowState = WindowState.Normal;
            WindowInstance.Activate();
        }

        public static void ShowWindow()
        {
            ShowWindow(null);
        }

        public CharacterWindow()
        {
            InitializeComponent();
            inventoryView.ItemDropCallback = InventoryDrop;
            characterView.ItemDropCallback = InventoryDrop;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            WindowInstance = null;
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await Refresh();
        }

        private async Task Refresh()
        {
            var characterData = await(ApiClient.Connection?.GetAsync<CharacterData>("/api/Player/gogomantv/shishka") ?? throw new NetworkException("No connection"));
            FillData(characterData);
        }

        private void FillData(CharacterData characterData)
        {
            characterView.FillData(characterData);
            inventoryView.FillData(characterData.Inventory);
        }

        private void InventoryDrop(InventoryItemContext source, InventoryItemContext target)
        {
            if (source.Owner == inventoryView && target.Owner == inventoryView)
            {
                SwapServerCall(source, target);
                return;
            }
            else if (source.Owner == inventoryView && target.Owner == characterView)
            {
                EquipServerCall(source,target);
            }
            else if (source.Owner == characterView && target.Owner == inventoryView)
            {
                EquipServerCall(target, source);
            }
            else
                Debug.Assert(false);
        }

        private async void SwapServerCall(InventoryItemContext source, InventoryItemContext target)
        {
            var parameters1 = new Dictionary<string, string>
            {
                ["sourceItem"] = source.Id.ToString(),
                ["targetItem"] = target.Id.ToString(),
            };
            var newInventory1 = await (ApiClient.Connection?.PostAsync<InventoryData>("/api/Player/gogomantv/shishka/inventoryswap", parameters1.ToImmutableDictionary()) ?? throw new NetworkException("No connection"));
            inventoryView.FillData(newInventory1);
        }

        private async void EquipServerCall(InventoryItemContext itemToEquip, InventoryItemContext itemToUnequip)
        {
            var parameters = new Dictionary<string, string>
            {
                ["itemToEquip"] = itemToEquip.Id.ToString(),
                ["itemToUnequip"] = itemToUnequip.Id.ToString(),
            };
            var characterData = await (ApiClient.Connection?.PostAsync<CharacterData>("/api/Player/gogomantv/shishka/equipitem", parameters.ToImmutableDictionary()) ?? throw new NetworkException("No connection"));
            FillData(characterData);
        }
    }
}
