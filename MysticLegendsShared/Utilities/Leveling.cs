namespace MysticLegendsShared.Utilities;

public static class Leveling
{
    public static int GetXpToLevelUp(int level)
    {
        return (int)Math.Pow(level, 3) + 10;
    }

    public static bool LevelUpIfPossible(ref int level, ref int xp)
    {
        var xpRequiredForLevelUp = GetXpToLevelUp(level);

        var loopLooped = xp >= xpRequiredForLevelUp;
        while (xp >= xpRequiredForLevelUp)
        {
            level++;
            xp -= xpRequiredForLevelUp;
            xpRequiredForLevelUp = GetXpToLevelUp(level);
        }
        return loopLooped;
    }
}
