namespace MysticLegendsClasses
{
    public struct BattleStat
    {
        public enum Method : uint
        {
            Add,
            Multiply,
            Fix,
        }

        public enum Type : uint
        {
            Strength = 0,
            Dexterity,
            Intelligence,

            PhysicalDamage = 10,
            Swiftness,
            MagicStrength,

            Resilience = 20,
            Evade,
            MagicProtection,

            FireDamage = 30,
            FireResistance,

            PoisonDamage,
            PoisonResistance,

            ArcaneStrength,
            ArcaneResistance,
        }

        public Method BattleStatMethod { get; set; }
        public Type BattleStatType { get; set; }
        public double Value { get; set; }
    }
}
