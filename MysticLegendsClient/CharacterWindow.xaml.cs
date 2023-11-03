using MysticLegendsShared.Models;
using System.Collections.Immutable;
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
            Character characterData = await GameState.Current.Connection.GetAsync<Character>("/api/Character/zmrdus");
            FillData(characterData);
        }

        private void FillData(Character characterData)
        {
            characterView.Data = characterData;
            if (characterData.CharacterInventory is not null)
                inventoryView.Data = characterData.CharacterInventory;
        }

        private void EquipSwapCheckExec(InventoryItem inventoryItem, InventoryItem equipedItem)
        {
            if (inventoryItem.Item.ItemType == equipedItem.Item.ItemType)
                EquipSwapServerCall(inventoryItem.InvitemId);
        }
        private void InventoryDrop(ItemDropContext source, ItemDropContext target)
        {
            if (source.Owner == inventoryView && target.Owner == inventoryView)
            {
                var inventoryItem = inventoryView.GetByContextId(source.Id);
                SwapServerCall(inventoryItem!.InvitemId, target.Id);
            }

            else if (source.Owner == inventoryView && target.Owner == characterView)
            {
                var inventoryItem = inventoryView.GetByContextId(source.Id);
                var equipedItem = characterView.GetByContextId(target.Id);

                if (inventoryItem is not null && equipedItem is not null)
                    EquipSwapCheckExec(inventoryItem, equipedItem);

                else if (inventoryItem is not null)
                    EquipServerCall(inventoryItem.InvitemId);
            }
            else if (source.Owner == characterView && target.Owner == inventoryView)
            {
                var inventoryItem = inventoryView.GetByContextId(target.Id);
                var equipedItem = characterView.GetByContextId(source.Id);
                if (inventoryItem is not null && equipedItem is not null)
                    EquipSwapCheckExec(inventoryItem, equipedItem);

                else if (equipedItem is not null)
                    UnequipServerCall(equipedItem.InvitemId, target.Id);
            }
        }

        private async void SwapServerCall(int itemId, int position)
        {
            var parameters1 = new Dictionary<string, string>
            {
                ["itemId"] = itemId.ToString(),
                ["position"] = position.ToString(),
            };
            var newInventory1 = await GameState.Current.Connection.PostAsync<CharacterInventory>("/api/Character/zmrdus/inventory-swap", parameters1.ToImmutableDictionary());
            inventoryView.Data = newInventory1;
        }

        private async void EquipServerCall(int itemToEquip)
        {
            var parameters = new Dictionary<string, string>
            {
                ["equipItemId"] = itemToEquip.ToString(),
            };
            var characterData = await GameState.Current.Connection.PostAsync<Character>("/api/Character/zmrdus/equip-item", parameters.ToImmutableDictionary());
            FillData(characterData);
        }

        private async void UnequipServerCall(int itemToUnequip, int? position)
        {
            var parameters = new Dictionary<string, string>
            {
                ["unequipItemId"] = itemToUnequip.ToString(),
            };
            if (position is not null)
                parameters["position"] = position.ToString()!;
            var characterData = await GameState.Current.Connection.PostAsync<Character>("/api/Character/zmrdus/unequip-item", parameters.ToImmutableDictionary());
            FillData(characterData);
        }

        private async void EquipSwapServerCall(int itemToSwapEquip)
        {
            var parameters = new Dictionary<string, string>
            {
                ["equipItemId"] = itemToSwapEquip.ToString(),
            };
            var characterData = await GameState.Current.Connection.PostAsync<Character>("/api/Character/zmrdus/swap-equip-item", parameters.ToImmutableDictionary());
            FillData(characterData);
        }
    }
}
