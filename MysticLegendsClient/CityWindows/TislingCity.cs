namespace MysticLegendsClient.CityWindows;

internal class TislingCity: CityWindow
{
    public TislingCity() : base("Tisling")
    {
        SetSplashImage("/images/Cities/Tisling.png");
        ShowButtons(new ButtonType[] {
                ButtonType.Storage,
                ButtonType.Blacksmith,
                ButtonType.Potions,
                ButtonType.TradeMarket,
                ButtonType.Scout,
        });
    }
}
