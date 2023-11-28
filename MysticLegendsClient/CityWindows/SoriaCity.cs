namespace MysticLegendsClient.CityWindows;

internal class SoriaCity: CityWindow
{
    public SoriaCity() : base("Soria")
    {
        SetSplashImage("/images/Cities/Soria.png");
        ShowButtons(new ButtonType[] {
                ButtonType.Storage,
                ButtonType.Blacksmith,
                ButtonType.Potions,
                ButtonType.TradeMarket,
        });
    }
}
