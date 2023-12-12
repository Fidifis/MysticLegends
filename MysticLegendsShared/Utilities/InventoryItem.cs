namespace MysticLegendsShared.Models;

public partial class InventoryItem
{
    public InventoryItem Clone()
    {
        var newItem = new InventoryItem()
        {
            //InvitemId = this.InvitemId, // let the database assign autoincrement
            CityName = CityName,
            CityInventoryCharacterName = CityInventoryCharacterName,
            CharacterName = CharacterName,
            CharacterInventoryCharacterN = CharacterInventoryCharacterN,
            ItemId = ItemId,
            NpcId = NpcId,
            StackCount = StackCount,
            Position = Position,
            Level = Level,
            Durability = Durability,
            //BattleStats = new List<BattleStat>(BattleStats), // there may be bug, because we keep invitemId of original, not the id of copied item. The stats may be attached to original.
        };
        newItem.BattleStats = new List<BattleStat>(BattleStats.Select(stat =>
        {
            var cloned = stat.Clone();
            cloned.InvitemId = -1;
            cloned.Invitem = newItem;
            return cloned;
        }));
        return newItem;
    }
}
