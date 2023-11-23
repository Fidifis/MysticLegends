using MysticLegendsClient.NpcWindows;
using MysticLegendsClient.Resources;
using System.Windows;

namespace MysticLegendsClient.CityWindows
{
    internal sealed class AyreimCity : CityWindow
    {
        private readonly SingleInstanceWindow queenOfAyreimWindow = new(typeof(QueenOfAyreimNpc), 4);

        public AyreimCity(): base("Ayreim")
        {
            SetSplashImage("/images/Cities/Ayreim.png");
            ShowButtons(new ButtonType[] {
                ButtonType.Storage,
                ButtonType.Blacksmith,
                ButtonType.Potions,
                ButtonType.TradeMarket,
                ButtonType.Scout,
                ButtonType.DarkAlley,
            });
            AddButton("Queen of Ayreim", Icons.city_crown, (object? s, RoutedEventArgs e) => { queenOfAyreimWindow.Instance.ShowWindow(); });
        }

        protected override void Window_Closed(object sender, EventArgs e)
        {
            base.Window_Closed(sender, e);
            queenOfAyreimWindow.Dispose();
        }
    }
}
