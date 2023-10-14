using System.Collections.Immutable;

namespace MysticLegendsClasses
{
    using MutableStatsType = Dictionary<BattleStat.Type, BattleStat>;
    using ImmutableStatsType = ImmutableDictionary<BattleStat.Type, BattleStat>;

    public class BattleStats
    {
        public ImmutableStatsType Stats { get; set; } = ImmutableDictionary.Create<BattleStat.Type, BattleStat>();

        public BattleStats() { }

        public BattleStats(MutableStatsType stats)
        {
            Stats = stats.ToImmutableDictionary();
        }

        public BattleStats(IEnumerable<BattleStat> stats)
        {
            var mutableStats = new MutableStatsType();
            foreach (var stat in stats)
            {
                if (mutableStats.ContainsKey(stat.BattleStatType))
                    throw new InvalidOperationException("BattleStat array can't contain two BattleStat with same BattleStat.Type");
                mutableStats[stat.BattleStatType] = stat;
            }
            Stats = mutableStats.ToImmutableDictionary();
        }

        public BattleStats(IEnumerable<BattleStats> external)
        {
            var mutableStats = new MutableStatsType();
            MutateStats(ref mutableStats, external);
            Stats = mutableStats.ToImmutableDictionary();
        }

        public BattleStats ApplyExternalStats(IEnumerable<BattleStats> external)
        {
            var mutableStats = new MutableStatsType(Stats);
            MutateStats(ref mutableStats, external);
            return new(mutableStats);
        }

        // NOTE: ref is here to make it clear what is mutable
        private static void MutateStats(ref MutableStatsType internalStats, IEnumerable<BattleStats> external)
        {
            MutateStatsByMethod(ref internalStats, external, BattleStat.Method.Add);
            MutateStatsByMethod(ref internalStats, external, BattleStat.Method.Multiply);
        }

        private static void MutateStatsByMethod(ref MutableStatsType internalStats, IEnumerable<BattleStats> external, BattleStat.Method method)
        {
            foreach (var externalBattleStat in external)
            {
                foreach (var externalStat in externalBattleStat.Stats.Values)
                {
                    if (externalStat.BattleStatMethod != method)
                        continue;

                    BattleStat battleStat = !internalStats.ContainsKey(externalStat.BattleStatType)
                        ? new BattleStat() { BattleStatType = externalStat.BattleStatType }
                        : internalStats[externalStat.BattleStatType];

                    switch (method)
                    {
                        case BattleStat.Method.Base:
                        case BattleStat.Method.Add:
                            battleStat.Value += externalStat.Value;
                            break;

                        case BattleStat.Method.Multiply:
                            battleStat.Value *= externalStat.Value;
                            break;
                    }

                    internalStats[battleStat.BattleStatType] = battleStat;
                }
            }
        }
    }
}
