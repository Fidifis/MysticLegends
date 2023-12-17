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
        MobId = MobId,
        Mob = Mob,
    };

    public BattleStat() { }

    private BattleStat(CBattleStat battleStat)
    {
        StatType = (int)battleStat.StatType;
        Method = (int)battleStat.StatMethod;
        Value = battleStat.Value;
    }

    public BattleStat(CBattleStat battleStat, InventoryItem linkedItem): this(battleStat)
    {
        Invitem = linkedItem;
        InvitemId = linkedItem.InvitemId;
    }

    public BattleStat(CBattleStat battleStat, Mob linkedmob) : this(battleStat)
    {
        MobId = linkedmob.MobId;
        Mob = linkedmob;
    }
}
