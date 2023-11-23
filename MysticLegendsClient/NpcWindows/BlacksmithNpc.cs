namespace MysticLegendsClient.NpcWindows;

internal sealed class BlacksmithNpc : NpcShopWindow
{
    public BlacksmithNpc(int npcId) : base(npcId)
    {
        SetSplashImage("/images/NPCs/blacksmith.png");
    }
}
