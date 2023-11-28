namespace MysticLegendsClient.CityWindows;

internal class DagosCity: CityWindow
{
    public DagosCity() : base("Dagos")
    {
        SetSplashImage("/images/Cities/Dagos.png");
        ShowButtons(new ButtonType[] {
                ButtonType.Storage,
                ButtonType.Blacksmith,
                ButtonType.Potions,
        });
    }
}
