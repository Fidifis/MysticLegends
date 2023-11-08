using MysticLegendsClient.Controls;
using MysticLegendsShared.Models;
using MysticLegendsShared.Utilities;
using System.Windows;

namespace MysticLegendsClient
{
    /// <summary>
    /// Interakční logika pro NpcWindow.xaml
    /// </summary>
    public abstract partial class NpcShopWindow : NpcWindow
    {
        
        protected readonly FrameworkElement[] views;

        protected InventoryView? inventoryRelation;
        protected CharacterWindow? characterWindowRelation;

        public NpcShopWindow(int npcId): base (npcId)
        {
            InitializeComponent();
            views = new FrameworkElement[] { buyView, sellView, questsView };

            buyView.ItemDropEvent += ItemDropBuy;
            sellViewInventory.ItemDropEvent += ItemDropSell;

            sellViewInventory.CanTransitItems = true;
        }

        protected override void SetSplashImage(string image)
        {
            splashImage.Source = BitmapTools.FromResource(image);
        }

        protected void Window_Loaded(object sender, RoutedEventArgs e)
        {
            BuyButton_Click(null, null);
        }

        protected void ChangeToView(FrameworkElement toShow)
        {
            IItemView.CloseTransition(sellViewInventory);
            foreach (var element in views)
            {
                if (element == toShow)
                    toShow.Visibility = Visibility.Visible;
                else
                    element.Visibility = Visibility.Hidden;
            }
        }

        protected void ItemDropBuy(IItemView sender, ItemDropEventArgs args)
        {
            if (sender == buyView && args.ToSlot == args.FromSlot)
            {
                var response = MessageBox.Show("Do you want to buy this item?", "buy", MessageBoxButton.YesNo);
            }

            else if (args.IsHandover)
            {
                // TODO: server call buy
            }
        }
        protected void ItemDropSell(IItemView sender, ItemDropEventArgs args)
        {
            if (sender == sellViewInventory)
            {
                // Moving item within sellView
                var item = args.FromSlot.Item;
                item!.Position = args.ToSlot.GridPosition;
                sellViewInventory.Update();
            }
            else if (args.IsHandover)
            {
                // Item leaves sell grid and new position in inventory is set

                sender.InvokeItemDropEvent(sender, new ItemDropEventArgs(sender.GetRelationBySlot(args.FromSlot)!.ManagedSlot, args.ToSlot));
                IItemView.CloseTransition(sellViewInventory);
                // or do a server call for inventory swap
            }
            else
            {
                // remove from inventory view, add to sell grid
                var itemCopy = PartialItemCopy(args.FromSlot.Item!);
                itemCopy.Position = args.ToSlot.GridPosition;
                // TODO: REWORK!!!
                var _velky_spatny = sellViewInventory.Items;
                _velky_spatny.Add(itemCopy);
                sellViewInventory.Items = _velky_spatny;
                sellViewInventory.Update();
                IItemView.EstablishRelation(args.FromSlot, args.ToSlot);
            }
        }

        protected static InventoryItem PartialItemCopy(InventoryItem item)
        {
            return new InventoryItem() { InvitemId = item.InvitemId, Item = item.Item, StackCount = item.StackCount, Position = item.Position };
        }

        protected async Task<List<NpcItem>> GetOfferedItemsAsync()
        {
            return await GameState.Current.Connection.GetAsync<List<NpcItem>>($"api/NpcShop/{NpcId}/offered-items");
        }

        protected async void BuyButton_Click(object? sender, RoutedEventArgs? e)
        {
            ChangeToView(buyView);
            var items = await GetOfferedItemsAsync();

            // TODO: Workaround - implement ability to process npc items
            var __convertedItems = new List<InventoryItem>();
            items.ForEach(item => __convertedItems.Add(item.InventoryItem!));

            buyView.Items = items.Select(item => item.InventoryItem!).ToList();
        }

        protected void SellButton_Click(object sender, RoutedEventArgs e)
        {
            ChangeToView(sellView);

            sellViewInventory.FillData(new List<InventoryItem>(), 20);
        }

        protected void QuestsButton_Click(object sender, RoutedEventArgs e)
        {
            ChangeToView(questsView);
        }

        protected virtual void Window_Closed(object sender, EventArgs e)
        {
            IItemView.CloseTransition(sellViewInventory);
        }
    }
}
