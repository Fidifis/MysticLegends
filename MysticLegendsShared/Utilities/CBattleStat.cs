using MysticLegendsShared.Models;

namespace MysticLegendsShared.Utilities;

public record struct CBattleStat
{
    public enum Method
    {
        Add,
        Multiply,
        Fix,
    }

    public enum Type
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

    public Method StatMethod { get; set; }
    public Type StatType { get; set; }
    public double Value { get; set; }

    public CBattleStat() { }

    public CBattleStat(Method method, Type type, double value)
    {
        StatMethod = method;
        StatType = type;
        Value = value;
    }

    public CBattleStat(BattleStat battleStat)
    {
        StatMethod = (Method)battleStat.Method;
        StatType = (Type)battleStat.StatType;
        Value = battleStat.Value;
    }

    public static IEnumerable<CBattleStat> ConvertFromBattleStat(IEnumerable<BattleStat> battleStats) => from bs in battleStats select new CBattleStat(bs);

    public override string ToString() => StatMethod switch
    {
        Method.Add => string.Format("Adds {0:N2} {1}", Value, StatType),
        Method.Multiply => string.Format("Multiplies {0} by {1:N2}", StatType, Value),
        Method.Fix => string.Format("Fixes {0} at {1:N2}", StatType, Value),
        _ => "Unknown method"
    };
}
