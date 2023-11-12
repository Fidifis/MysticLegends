namespace MysticLegendsShared.Models;

public partial class InventoryItem
{
    public InventoryItem Clone()
    {
        return new InventoryItem()
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
            BattleStats = new List<BattleStat>(BattleStats),
        };
    }
}
