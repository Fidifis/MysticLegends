using MysticLegendsShared.Utilities;

namespace MysticLegendsServer;

public static class BattleStatsGenerator
{
    private const int StatFactor = 10;
    public static IEnumerable<CBattleStat> MakeBattleStats(IRNG rng, int itemLevel)
    {
        var numberOfStats = rng.RandomNumber(1, 6);

        return Enumerable.Range(0, numberOfStats).Select(_ => MakeStat(rng, itemLevel));
    }

    private static CBattleStat MakeStat(IRNG rng, int itemLevel)
    {
        CBattleStat.Method method = (CBattleStat.Method)rng.RandomNumber(3);

        var types = Enum.GetValues<CBattleStat.Type>();
        CBattleStat.Type type = types[rng.RandomNumber(types.Length)];

        double value = method switch
        {
            CBattleStat.Method.Add => rng.RandomDecimal(Math.Sqrt(itemLevel), itemLevel * StatFactor),
            CBattleStat.Method.Multiply => rng.RandomDecimal(0.8, Math.Sqrt(itemLevel)),
            CBattleStat.Method.Fix => rng.RandomDecimal(itemLevel * 2, itemLevel < StatFactor * 2 ? itemLevel * StatFactor : (itemLevel * itemLevel / 2)),
            _ => 0.0
        };

        return new CBattleStat(method, type, value);
    }
}
