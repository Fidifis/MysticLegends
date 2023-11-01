using System.Collections;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using MysticLegendsShared.Models;

namespace MysticLegendsShared.Utilities;

using StatsType = Dictionary<CBattleStat.Type, CBattleStat>;

public class BattleStats: IReadOnlyDictionary<CBattleStat.Type, CBattleStat>
{
    private readonly StatsType battleStats = new();

    public IEnumerable<CBattleStat.Type> Keys => battleStats.Keys;
    public IEnumerable<CBattleStat> Values => battleStats.Values;
    public int Count => battleStats.Count;

    public CBattleStat this[CBattleStat.Type key] => battleStats[key];

    public BattleStats() { }

    public BattleStats(StatsType stats) => battleStats = new StatsType(stats);

    public BattleStats(IEnumerable<BattleStat> stats)
    {
        foreach (var stat in CBattleStat.ConvertFromBattleStat(stats))
        {
            if (battleStats.ContainsKey(stat.StatType))
                throw new InvalidOperationException("BattleStat array can't contain two BattleStat with same BattleStat.Type");
            battleStats[stat.StatType] = stat;
        }
    }

    public BattleStats(IEnumerable<BattleStats> external)
    {
        MutateStats(battleStats, external);
    }

    private static void MutateStats(StatsType internalStats, IEnumerable<BattleStats> external)
    {
        MutateStatsByMethod(internalStats, external, CBattleStat.Method.Add);
        MutateStatsByMethod(internalStats, external, CBattleStat.Method.Multiply);
        MutateStatsByMethod(internalStats, external, CBattleStat.Method.Fix);
    }

    private static void MutateStatsByMethod(StatsType internalStats, IEnumerable<BattleStats> external, CBattleStat.Method method)
    {
        foreach (var externalBattleStat in external)
        {
            foreach (var externalStat in externalBattleStat.battleStats.Values)
            {
                if (externalStat.StatMethod != method)
                    continue;

                CBattleStat battleStat = internalStats.ContainsKey(externalStat.StatType)
                    ? internalStats[externalStat.StatType]
                    : new CBattleStat() { StatType = externalStat.StatType };

                Debug.Assert(battleStat.StatMethod == CBattleStat.Method.Add, "Internal stats must be of Add method");

                switch (method)
                {
                    case CBattleStat.Method.Add:
                        battleStat.Value += externalStat.Value;
                        break;

                    case CBattleStat.Method.Multiply:
                        battleStat.Value *= externalStat.Value;
                        break;

                    case CBattleStat.Method.Fix:
                        battleStat.Value =
                            battleStat.StatMethod == CBattleStat.Method.Fix
                            ? Math.Max(battleStat.Value, externalStat.Value)
                            : externalStat.Value;
                        break;
                }

                internalStats[battleStat.StatType] = battleStat;
            }
        }
    }

    public bool ContainsKey(CBattleStat.Type key) => battleStats.ContainsKey(key);
    public bool TryGetValue(CBattleStat.Type key, [MaybeNullWhen(false)] out CBattleStat value) => battleStats.TryGetValue(key, out value);
    public IEnumerator<KeyValuePair<CBattleStat.Type, CBattleStat>> GetEnumerator() => battleStats.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => battleStats.GetEnumerator();
}
