using System.Collections.Immutable;

namespace MysticLegendsClasses
{
    using MutableStatsType = Dictionary<BattleStat.Type, BattleStat>;
    using ImmutableStatsType = ImmutableDictionary<BattleStat.Type, BattleStat>;

    public readonly struct BattleStats
    {
        public ImmutableStatsType Stats { get; init; }

        public BattleStats()
        {
            Stats = ImmutableDictionary.Create<BattleStat.Type, BattleStat>();
        }

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
            MutateStats(mutableStats, external);
            Stats = mutableStats.ToImmutableDictionary();
        }

        public readonly BattleStats ApplyExternalStats(IEnumerable<BattleStats> external)
        {
            var mutableStats = new MutableStatsType(Stats);
            MutateStats(mutableStats, external);
            return new(mutableStats);
        }

        private static void MutateStats(MutableStatsType internalStats, IEnumerable<BattleStats> external)
        {
            MutateStatsByMethod(internalStats, external, BattleStat.Method.Add);
            MutateStatsByMethod(internalStats, external, BattleStat.Method.Multiply);
        }

        private static void MutateStatsByMethod(MutableStatsType internalStats, IEnumerable<BattleStats> external, BattleStat.Method method)
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
