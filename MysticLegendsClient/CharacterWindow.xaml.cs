using MysticLegendsShared.Models;
using MysticLegendsShared.Utilities;
using System.Collections.Immutable;
using System.Windows;

namespace MysticLegendsClient
{
    /// <summary>
    /// Interakční logika pro CharacterWindow.xaml
    /// </summary>
    public partial class CharacterWindow : Window, ISingleInstanceWindow
    {
        public void ShowWindow()
        {
            Show();
            if (WindowState == WindowState.Minimized) WindowState = WindowState.Normal;
            Activate();
        }

        public CharacterWindow()
        {
            InitializeComponent();
            characterView.ItemDropEvent += InventoryDropOnCharacter;
            inventoryView.ItemDropEvent += InventoryDropOnInventory;

            inventoryView.CanTransitItems = true;

            GameState.Current.GameEvents.CharacterInventoryUpdateEvent += (object? sender, CharacterInventoryUpdateEventArgs e) =>
                inventoryView.Items = e.InventoryItems.AsReadOnly();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Character characterData = await GameState.Current.Connection.GetAsync<Character>("/api/Character/zmrdus");
            FillData(characterData);
        }

        private void FillData(Character characterData)
        {
            characterView.FillData(characterData.CharacterName, characterData.InventoryItems.AsReadOnly());
            if (characterData.CharacterInventory is not null)
                inventoryView.FillData(characterData.CharacterInventory.InventoryItems.AsReadOnly(), characterData.CharacterInventory.Capacity);
        }

        private void EquipSwapCheckExec(InventoryItem inventoryItem, InventoryItem equipedItem)
        {
            if (inventoryItem.Item.ItemType == equipedItem.Item.ItemType)
                EquipSwapServerCall(inventoryItem.InvitemId);
        }
        private void InventoryDropOnCharacter(IItemView sender, ItemDropEventArgs args)
        {
            if (sender == inventoryView)
            {
                var inventoryItem = args.FromSlot.Item;
                var equipedItem = args.ToSlot.Item;

                if (inventoryItem is not null && equipedItem is not null)
                    EquipSwapCheckExec(inventoryItem, equipedItem);

                else if (inventoryItem is not null)
                    EquipServerCall(inventoryItem.InvitemId);
            }
        }

        private void InventoryDropOnInventory(IItemView sender, ItemDropEventArgs args)
        {
            if (sender == inventoryView)
            {
                SwapServerCall(args.FromSlot.Item!.InvitemId, args.ToSlot.GridPosition);
            }
            else if (sender == characterView)
            {
                var inventoryItem = args.ToSlot.Item;
                var equipedItem = args.FromSlot.Item;
                if (inventoryItem is not null && equipedItem is not null)
                    EquipSwapCheckExec(inventoryItem, equipedItem);

                else if (equipedItem is not null)
                    UnequipServerCall(equipedItem.InvitemId, args.ToSlot.GridPosition);
            }
            else
                IItemView.DropEventHandover(args);
        }

        private async void SwapServerCall(int itemId, int position)
        {
            var parameters1 = new Dictionary<string, string>
            {
                ["itemId"] = itemId.ToString(),
                ["position"] = position.ToString(),
            };
            var newInventory1 = await GameState.Current.Connection.PostAsync<CharacterInventory>("/api/Character/zmrdus/inventory-swap", parameters1.ToImmutableDictionary());
            inventoryView.Items = newInventory1.InventoryItems.AsReadOnly();
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
