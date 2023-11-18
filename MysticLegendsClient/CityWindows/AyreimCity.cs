using MysticLegendsClient.Resources;
using System.Windows;

namespace MysticLegendsClient.CityWindows
{
    internal sealed class AyreimCity : CityWindow
    {
        private readonly SingleInstanceWindow<CharacterWindow> ayreimMarketWindow = new();

        public AyreimCity(): base("Ayreim")
        {
            SetSplashImage("/images/Cities/Ayreim.png");
            ShowButtons(new ButtonType[] {
                ButtonType.Blacksmith,
                ButtonType.Potions,
                ButtonType.TradeMarket,
                ButtonType.Scout,
                ButtonType.DarkAlley,
            });
            AddButton("Ayreim Market", Icons.city_coins, (object? s, RoutedEventArgs e) => { });
        }

        protected override void Window_Closed(object sender, EventArgs e)
        {
            base.Window_Closed(sender, e);
            ayreimMarketWindow.Dispose();
        }
    }
}
