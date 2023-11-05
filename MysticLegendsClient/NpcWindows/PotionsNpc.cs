namespace MysticLegendsClient.NpcWindows;

internal sealed class PotionsNpc: NpcWindow
{
    public PotionsNpc(int npcId) : base (npcId)
    {
        SetSplashImage("/images/NPCs/potion_crafter.png");
    }
}
