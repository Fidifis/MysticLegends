namespace MysticLegendsClasses
{
    public struct BattleStats
    {
        public int Strength { get; set; }
        public int Dexterity { get; set; }
        public int Intelligence { get; set; }

        public int PhysicalDamage { get; set; }
        public int Swiftness { get; set; }
        public int MagicStrength { get; set; }

        public int Resilience { get; set; }
        public int Evade { get; set; }
        public int MagicProtection { get; set; }

        public int FireDamage { get; set; }
        public int ColdStrength { get; set; }
        public int PoisonDamage { get; set; }
        public int ArcaneStrength { get; set; }

        public int FireResistance { get; set; }
        public int ColdResistance { get; set; }
        public int PoisonResistance { get; set; }
        public int ArcaneResistance { get; set; }

        public static BattleStats Sum(IEnumerable<BattleStats> stats)
        {
            BattleStats sum = new();
            Type battleStatsType = typeof(BattleStats);

            foreach (var item in stats)
            {
                foreach (var property in battleStatsType.GetProperties())
                {
                    if (property.PropertyType == typeof(int))
                    {
                        int currentValue = (int?)property.GetValue(sum) ?? 0;
                        int itemValue = (int?)property.GetValue(item) ?? 0;
                        property.SetValue(sum, currentValue + itemValue);
                    }
                }
            }
            return sum;
        }
    }
}
