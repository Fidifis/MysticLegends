using MysticLegendsShared.Models;
using MysticLegendsShared.Utilities;
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

            GameState.Current.GameEvents.CharacterInventoryUpdateEvent += (object? sender, UpdateEventArgs<IReadOnlyCollection<InventoryItem>> e) =>
                inventoryView.Items = e.Value;

            GameState.Current.GameEvents.CharacterUpdateEvent += (object? sender, UpdateEventArgs<Character> e) =>
                FillData(e.Value);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ErrorCatcher.Try(async () =>
            {
                Character characterData = await ApiCalls.CharacterCall.GetCharacterServerCallAsync("zmrdus");
                FillData(characterData);
            });
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
                ApiCalls.CharacterCall.EquipSwapServerCall(this, inventoryItem.InvitemId);
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
                    ApiCalls.CharacterCall.EquipServerCall(this, inventoryItem.InvitemId);
            }
        }

        private void InventoryDropOnInventory(IItemView sender, ItemDropEventArgs args)
        {
            if (sender == inventoryView)
            {
                ApiCalls.CharacterCall.SwapServerCall(this, args.FromSlot.Item!.InvitemId, args.ToSlot.GridPosition);
            }
            else if (sender == characterView)
            {
                var inventoryItem = args.ToSlot.Item;
                var equipedItem = args.FromSlot.Item;
                if (inventoryItem is not null && equipedItem is not null)
                    EquipSwapCheckExec(inventoryItem, equipedItem);

                else if (equipedItem is not null)
                    ApiCalls.CharacterCall.UnequipServerCall(this, equipedItem.InvitemId, args.ToSlot.GridPosition);
            }
            else
                IItemView.DropEventHandover(args);
        }
    }
}
