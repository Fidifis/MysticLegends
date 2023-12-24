using MysticLegendsClient.Controls;
using MysticLegendsShared.Models;
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
            splashImage.Source = BitmapTools.ImageFromResource(image);
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

        protected void UpdateSellPrice()
        {
            ErrorCatcher.Try(async () =>
            {
                var price = await ApiCalls.NpcCall.GetOfferedPriceServerCallAsync(NpcId, sellViewInventory.Items);
                priceTextBox.Text = price.ToString();
            });
        }

        protected void ItemDropBuy(IItemView sender, ItemDropEventArgs args)
        {
            if (sender == buyView && args.ToSlot == args.FromSlot)
            {
                // buy item by clicking
                var response = MessageBox.Show("Do you want to buy this item?", "buy", MessageBoxButton.YesNo);
                if (response == MessageBoxResult.Yes)
                    BuyItem(args.FromSlot.Item!.InvitemId, args.ToSlot.GridPosition);
            }

            else if (args.IsHandover)
            {
                // buy item by drag to inventory
                BuyItem(args.FromSlot.Item!.InvitemId, args.ToSlot.GridPosition);
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

        protected async void BuyItem(int invitemId, int position)
        {
            await ApiCalls.NpcCall.BuyItemServerCallAsync(this, NpcId, invitemId, position);
            ApiCalls.CharacterCall.UpdateCharacter(this, GameState.Current.CharacterName);
            await RefreshBuyView();
        }

        protected static InventoryItem PartialItemCopy(InventoryItem item)
        {
            return new InventoryItem() { InvitemId = item.InvitemId, Item = item.Item, BattleStats = item.BattleStats, StackCount = item.StackCount, Position = item.Position };
        }

        protected async Task RefreshBuyView()
        {
            await ErrorCatcher.TryAsync(async () =>
            {
                buyView.Items = await ApiCalls.NpcCall.GetOfferedItemsServerCallAsync(NpcId);
            });
        }

        protected async Task RefreshQuestView()
        {
            await ErrorCatcher.TryAsync(async () =>
            {
                var quests = await ApiCalls.NpcQuestCall.GetOfferedQuestsServerCallAsync(NpcId, GameState.Current.CharacterName);
                questsView.FillData(quests);
            });
        }

        protected async void BuyButton_Click(object? sender, RoutedEventArgs? e)
        {
            ChangeToView(buyView);
            await RefreshBuyView();
        }

        protected void SellButton_Click(object? sender, RoutedEventArgs? e)
        {
            ChangeToView(sellView);

            sellViewInventory.FillData(new List<InventoryItem>(), 18);
        }

        protected async void QuestsButton_Click(object? sender, RoutedEventArgs? e)
        {
            ChangeToView(questsView);
            await RefreshQuestView();
        }

        protected virtual void Window_Closed(object sender, EventArgs e)
        {
            sellViewInventory.CloseRelations();
        }

        private async void MakeSell_Click(object sender, RoutedEventArgs e)
        {
            await ApiCalls.NpcCall.SellItemsServerCallAsync(this, NpcId, sellViewInventory.Items);
            ApiCalls.CharacterCall.UpdateCharacter(this, GameState.Current.CharacterName);
            sellViewInventory.CloseRelations();
            sellViewInventory.Items = new List<InventoryItem>();
            priceTextBox.Text = "0";
        }
    }
}
