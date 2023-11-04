using MysticLegendsClient.Controls;
using MysticLegendsShared.Models;
using MysticLegendsShared.Utilities;
using System.Diagnostics;
using System.Windows;

namespace MysticLegendsClient
{
    /// <summary>
    /// Interakční logika pro NpcWindow.xaml
    /// </summary>
    public partial class NpcWindow : Window, ISingleInstanceWindow
    {
        private readonly FrameworkElement[] views;

        private InventoryView? inventoryRelation;

        public NpcWindow(NpcType npcType)
        {
            InitializeComponent();
            views = new FrameworkElement[] { buyView, sellView, questsView };

            SetSplashImage(npcType);

            buyView.ItemDropSourceCallback = ItemDrop;
            sellViewInventory.ItemDropTargetCallback = ItemDrop;
        }

        public void ShowWindow()
        {
            Show();
            if (WindowState == WindowState.Minimized) WindowState = WindowState.Normal;
            Activate();
        }

        private void SetSplashImage(NpcType npcType)
        {
            splashImage.Source = BitmapTools.FromResource(npcType switch
            {
                NpcType.PotionsCrafter => "/images/NPCs/potion_crafter.png",
                _ => throw new NotImplementedException("requested npc type not implemented")
            });
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            BuyButton_Click(null, null);
        }

        private void ChangeToView(FrameworkElement toShow)
        {
            inventoryRelation?.ReleaseLock(this);
            foreach (var element in views)
            {
                if (element == toShow)
                    toShow.Visibility = Visibility.Visible;
                else
                    element.Visibility = Visibility.Hidden;
            }
        }

        private void ItemDrop(ItemDropContext source, ItemDropContext target)
        {
            InventoryView? inventoryView;
            if (source.Owner == buyView && target.Owner == buyView &&
                source.ContextId == target.ContextId)
            {
                var response = MessageBox.Show("Do you want to buy this item?", "buy", MessageBoxButton.YesNo);
            }

            else if (source.Owner == buyView && (target.Owner as InventoryView)?.Owner as CharacterWindow is not null)
            {
                // TODO: server call buy
            }

            else if (source.Owner == sellViewInventory && (target.Owner as InventoryView)?.Owner as CharacterWindow is not null)
            {
                // TODO: server call inventory swap (item leaves sell grid and new position in inventory is set)
            }

            else if (target.Owner == sellViewInventory && (inventoryView = source.Owner as InventoryView)?.Owner as CharacterWindow is not null)
            {
                // TODO: tmp remove from inventory view, add to sell grid
                var movingItem = inventoryView.GetByContextId(source.ContextId)!;
                inventoryView.LockItem(this, movingItem.InvitemId);
                Debug.Assert(inventoryRelation is null || inventoryRelation == inventoryView);
                inventoryRelation = inventoryView;

                var itemCopy = PartialItemCopy(movingItem);
                itemCopy.Position = target.ContextId;
                sellViewInventory.Data!.InventoryItems.Add(PartialItemCopy(itemCopy));
                sellViewInventory.Update();
            }
            else if (source.Owner == sellViewInventory && target.Owner == sellViewInventory)
            {
                var item = sellViewInventory.GetByContextId(source.ContextId);
                item!.Position = target.ContextId;
                sellViewInventory.Update();
            }
        }

        private static InventoryItem PartialItemCopy(InventoryItem item)
        {
            return new InventoryItem() { InvitemId = item.InvitemId, Item = item.Item, StackCount = item.StackCount, Position = item.Position };
        }

        private void BuyButton_Click(object? sender, RoutedEventArgs? e)
        {
            ChangeToView(buyView);
            // TODO: fetch data
            var inv = new ArtifficialInventory
            {
                InventoryItems = new List<InventoryItem>() {
                new() { StackCount = 10, Item = new Item() { Icon = "bodyArmor/ayreimWarrior" } },
                new() { StackCount = 10, Item = new Item() { Icon = "helmet/ayreimWarrior"} },
                }
            };

            buyView.Data = inv;
        }

        private void SellButton_Click(object sender, RoutedEventArgs e)
        {
            ChangeToView(sellView);

            sellViewInventory.Data = new ArtifficialInventory { Capacity = 20 };
        }

        private void QuestsButton_Click(object sender, RoutedEventArgs e)
        {
            ChangeToView(questsView);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            inventoryRelation?.ReleaseLock(this);
        }
    }
}
