namespace MysticLegendsClient.NpcWindows;

internal sealed class DarkAlleyNpc : NpcShopWindow
{
    public DarkAlleyNpc(int npcId) : base(npcId)
    {
        SetSplashImage("/images/NPCs/dark_alley.png");
    }
}
