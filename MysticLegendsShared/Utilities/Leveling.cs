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

        if (xp >= xpRequiredForLevelUp)
        {
            level++;
            xp -= xpRequiredForLevelUp;

            return true;
        }
        return false;
    }
}
