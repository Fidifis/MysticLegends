using MysticLegendsShared.Utilities;

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

    public BattleStat() { }

    public BattleStat(CBattleStat battleStat, InventoryItem linkedItem)
    {
        StatType = (int)battleStat.StatType;
        Method = (int)battleStat.StatMethod;
        InvitemId = linkedItem.InvitemId;
        Invitem = linkedItem;
        Value = battleStat.Value;
    }
}
