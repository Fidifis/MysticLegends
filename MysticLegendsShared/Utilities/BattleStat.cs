namespace MysticLegendsShared.Models;

public partial class BattleStat
{
    public BattleStat Clone() => new()
    {
        StatType = StatType,
        Method = Method,
        InvitemId = InvitemId,
        Value = Value,
        Invitem = Invitem,
    };
}
