namespace MysticLegendsClient.NpcWindows;

// TODO: Maybe use different base class
internal sealed class QueenOfAyreimNpc : NpcShopWindow
{
    public QueenOfAyreimNpc(int npcId) : base(npcId)
    {
        SetSplashImage("/images/NPCs/queen_of_ayreim.png");
    }
}
