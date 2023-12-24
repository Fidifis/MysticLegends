using System.Windows;

namespace MysticLegendsClient.NpcWindows;

internal class RelicTraderWindow : NpcShopWindow
{
    public RelicTraderWindow(int npcId) : base(npcId)
    {
        SetSplashImage("/images/NPCs/relic_trader.png");
    }

    protected new void Window_Loaded(object sender, RoutedEventArgs e)
    {
        SellButton_Click(null, null);
        buyButton.Visibility = Visibility.Collapsed;
    }
}
