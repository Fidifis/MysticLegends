using MysticLegendsShared.Utilities;

namespace MysticLegendsServer
{
    public static class Fight
    {
        public static bool DecideFight(IRNG rng, BattleStats fighter, BattleStats opponent)
        {
            double totalScore = 0.0;
            foreach (var statType in Enum.GetValues<CBattleStat.Type>())
            {
                double fighterOffensive, fighterDeffensive, opponentOffensive, opponentDeffensive;

                var s1 = GetValuesOfBattleStat(statType, fighter, out fighterOffensive, out fighterDeffensive);
                var s2 = GetValuesOfBattleStat(statType, opponent, out opponentOffensive, out opponentDeffensive);

                if (s1 || s2)
                totalScore += OneFightScore(rng, fighterOffensive, fighterDeffensive, opponentOffensive, opponentDeffensive);
            }

            return totalScore >= 0;
        }

        private static bool GetValuesOfBattleStat(CBattleStat.Type statType, BattleStats battleStats,
            out double offensive, out double deffensive)
        {
            switch (statType)
            {
                case CBattleStat.Type.PhysicalDamage:
                case CBattleStat.Type.Resilience:
                    offensive = battleStats.Get(CBattleStat.Type.PhysicalDamage).Value;
                    deffensive = battleStats.Get(CBattleStat.Type.Resilience).Value;
                    break;
                case CBattleStat.Type.Swiftness:
                case CBattleStat.Type.Evade:
                    offensive = battleStats.Get(CBattleStat.Type.Swiftness).Value;
                    deffensive = battleStats.Get(CBattleStat.Type.Evade).Value;
                    break;
                case CBattleStat.Type.MagicStrength:
                case CBattleStat.Type.MagicProtection:
                    offensive = battleStats.Get(CBattleStat.Type.MagicStrength).Value;
                    deffensive = battleStats.Get(CBattleStat.Type.MagicProtection).Value;
                    break;
                case CBattleStat.Type.FireDamage:
                case CBattleStat.Type.FireResistance:
                    offensive = battleStats.Get(CBattleStat.Type.FireDamage).Value;
                    deffensive = battleStats.Get(CBattleStat.Type.FireResistance).Value;
                    break;
                case CBattleStat.Type.PoisonDamage:
                case CBattleStat.Type.PoisonResistance:
                    offensive = battleStats.Get(CBattleStat.Type.PoisonDamage).Value;
                    deffensive = battleStats.Get(CBattleStat.Type.PoisonResistance).Value;
                    break;
                case CBattleStat.Type.ArcaneStrength:
                case CBattleStat.Type.ArcaneResistance:
                    offensive = battleStats.Get(CBattleStat.Type.ArcaneStrength).Value;
                    deffensive = battleStats.Get(CBattleStat.Type.ArcaneResistance).Value;
                    break;
                default:
                    offensive = 0;
                    deffensive = 0;
                    return false;
            }
            return true;
        }

        private static double OneFightScore(IRNG rng,
            double fighterOffensive, double fighterDeffensive,
            double opponentOffensive, double opponentDeffensive)
        {
            var offense = fighterOffensive - opponentDeffensive;
            var deffense = opponentOffensive - fighterDeffensive;

            var score = offense - deffense;
            var scale = Math.Abs(offense) + Math.Abs(deffense);

            var shift = rng.RandomDecimal(scale);
            return score + shift - scale / 2;
        }
    }
}
