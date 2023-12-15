using MysticLegendsShared.Utilities;

namespace MysticLegendsServer
{
    public static class Fight
    {
        public static bool DecideFight(IRNG rng, BattleStats fighter, BattleStats opponent)
        {
            // TODO: implement
            return rng.RandomNumber(2) == 1;
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
