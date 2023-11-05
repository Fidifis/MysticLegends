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
    public abstract partial class NpcWindow : Window, ISingleInstanceWindow
    {
        protected readonly FrameworkElement[] views;

        protected InventoryView? inventoryRelation;
        protected CharacterWindow? characterWindowRelation;

        public NpcWindow()
        {
            InitializeComponent();
            views = new FrameworkElement[] { buyView, sellView, questsView };

            buyView.ItemDropSourceCallback = ItemDropSource;
            sellViewInventory.ItemDropSourceCallback = ItemDropSource;
            sellViewInventory.ItemDropTargetCallback = ItemDropTarget;
        }

        public void ShowWindow()
        {
            Show();
            if (WindowState == WindowState.Minimized) WindowState = WindowState.Normal;
            Activate();
        }

        protected void SetSplashImage(string image)
        {
            splashImage.Source = BitmapTools.FromResource(image);
        }

        protected void Window_Loaded(object sender, RoutedEventArgs e)
        {
            BuyButton_Click(null, null);
        }

        protected void ChangeToView(FrameworkElement toShow)
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

        protected void ItemDropSource(ItemDropContext source, ItemDropContext target)
        {
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
                var item = sellViewInventory.GetByContextId(source.ContextId)!;
                characterWindowRelation?.ReturnItem(this, item.InvitemId, target.ContextId);
                sellViewInventory.Data?.InventoryItems.Remove(item);
                sellViewInventory.Update();
            }
        }
        protected void ItemDropTarget(ItemDropContext source, ItemDropContext target)
        {
            InventoryView? inventoryView;

            if (target.Owner == sellViewInventory && (inventoryView = source.Owner as InventoryView)?.Owner is CharacterWindow characterWindow)
            {
                // TODO: tmp remove from inventory view, add to sell grid
                var movingItem = inventoryView.GetByContextId(source.ContextId)!;
                inventoryView.LockItem(this, movingItem.InvitemId);
                Debug.Assert(inventoryRelation is null || inventoryRelation == inventoryView);
                inventoryRelation = inventoryView;
                characterWindowRelation = characterWindow;

                var itemCopy = PartialItemCopy(movingItem);
                itemCopy.Position = target.ContextId;
                sellViewInventory.Data!.InventoryItems.Add(itemCopy);
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

        protected void BuyButton_Click(object? sender, RoutedEventArgs? e)
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

        protected void SellButton_Click(object sender, RoutedEventArgs e)
        {
            ChangeToView(sellView);

            sellViewInventory.Data = new ArtifficialInventory { Capacity = 20 };
        }

        protected void QuestsButton_Click(object sender, RoutedEventArgs e)
        {
            ChangeToView(questsView);
        }

        protected virtual void Window_Closed(object sender, EventArgs e)
        {
            inventoryRelation?.ReleaseLock(this);
        }
    }
}
