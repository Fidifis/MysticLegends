using MysticLegendsShared.Models;
using MysticLegendsShared.Utilities;
using System.Windows;

namespace MysticLegendsClient
{
    /// <summary>
    /// Interakční logika pro NpcWindow.xaml
    /// </summary>
    public partial class NpcWindow : Window, ISingleInstanceWindow
    {
        private readonly FrameworkElement[] views;

        public NpcWindow(NpcType npcType)
        {
            InitializeComponent();
            views = new FrameworkElement[] { buyView, sellView, questsView };

            SetSplashImage(npcType);

            buyView.ItemDropTargetCallback = ItemDrop;
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
            if (source.Owner == buyView && target.Owner == buyView &&
                source.ContextId == target.ContextId)
            {
                var response = MessageBox.Show("Do you want to buy this item?", "buy", MessageBoxButton.YesNo);
            }
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

            sellViewInventory.Data = new ArtifficialInventory { Capacity = 50 };
        }

        private void QuestsButton_Click(object sender, RoutedEventArgs e)
        {
            ChangeToView(questsView);
        }
    }
}
