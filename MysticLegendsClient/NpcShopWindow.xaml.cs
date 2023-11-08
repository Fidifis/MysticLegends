using MysticLegendsClient.Controls;
using MysticLegendsShared.Models;
using System.Text.Json;
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
            sellViewInventory.CloseRelations();
            foreach (var element in views)
            {
                if (element == toShow)
                    toShow.Visibility = Visibility.Visible;
                else
                    element.Visibility = Visibility.Hidden;
            }
        }

        protected async void UpdateSellPrice()
        {
            var price = await ApiCalls.NpcCall.GetOfferedPriceServerCallAsync(NpcId, sellViewInventory.Items);
            priceTextBox.Text = price.ToString();
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
                sellViewInventory.UpdateItem(item);
            }
            else if (args.IsHandover)
            {
                // Item leaves sell grid and new position in inventory is set
                var relationFromSlot = sender.GetRelationBySlot(args.FromSlot)!;
                sender.InvokeItemDropEvent(sender, new ItemDropEventArgs(relationFromSlot.ManagedSlot, args.ToSlot)); // or do a server call for inventory swap
                sellViewInventory.CloseRelation(relationFromSlot);
                UpdateSellPrice();
            }
            else
            {
                // remove from inventory view, add to sell grid
                var itemCopy = PartialItemCopy(args.FromSlot.Item!);
                itemCopy.Position = args.ToSlot.GridPosition;

                if (ItemViewRelation.EstablishRelation(args.FromSlot, args.ToSlot))
                    sellViewInventory.AddItem(itemCopy);

                UpdateSellPrice();
            }
        }

        protected static InventoryItem PartialItemCopy(InventoryItem item)
        {
            return new InventoryItem() { InvitemId = item.InvitemId, Item = item.Item, BattleStats = item.BattleStats, StackCount = item.StackCount, Position = item.Position };
        }

        protected async void BuyButton_Click(object? sender, RoutedEventArgs? e)
        {
            ChangeToView(buyView);
            var items = await ApiCalls.NpcCall.GetOfferedItemsServerCallAsync(NpcId);

            buyView.Items = items;
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
            sellViewInventory.CloseRelations();
        }

        private void MakeSell_Click(object sender, RoutedEventArgs e)
        {
            ApiCalls.NpcCall.SellItemsServerCall(this, NpcId, sellViewInventory.Items);
            ApiCalls.CharacterCall.UpdateCharacter(this, "zmrdus");
            ApiCalls.CharacterCall.UpdateCurrency(this, "zmrdus");
            sellViewInventory.CloseRelations();
            sellViewInventory.Items = new List<InventoryItem>();
        }
    }
}
