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
    }
}
