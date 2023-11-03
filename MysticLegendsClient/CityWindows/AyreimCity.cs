using MysticLegendsClient.Resources;

namespace MysticLegendsClient.CityWindows
{
    internal class AyreimCity : CityWindow
    {
        public AyreimCity(): base("Ayreim")
        {
            ShowButtons(new ButtonType[] { 
                ButtonType.Blacksmith,
                ButtonType.Potions,
                ButtonType.TradeMarket,
                ButtonType.Scout,
                ButtonType.DarkAlley,
            });
            AddButton("Ayreim Market", Icons.bar_gold);
        }
    }
}
