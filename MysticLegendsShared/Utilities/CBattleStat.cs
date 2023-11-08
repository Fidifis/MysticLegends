using MysticLegendsShared.Models;

namespace MysticLegendsShared.Utilities;

public struct CBattleStat
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

    public override readonly string ToString() => StatMethod switch
    {
        Method.Add => $"Adds {Value} {StatType}",
        Method.Multiply => $"Multiplies {StatType} by {Value}",
        Method.Fix => $"Fixes {StatType} at {Value}",
        _ => "Unknown method"
    };
}
