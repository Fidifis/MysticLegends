using System;
using System.Collections.Generic;

namespace MysticLegendsShared.Models;

public partial class InventoryItem
{
    public int InvitemId { get; set; }

    public string? CityName { get; set; }

    public string? CityInventoryCharacterName { get; set; }

    public string? CharacterName { get; set; }

    public string? CharacterInventoryCharacterN { get; set; }

    public int ItemId { get; set; }

    public int? NpcId { get; set; }

    public int StackCount { get; set; }

    public int Position { get; set; }

    public int? Level { get; set; }

    public int? Durability { get; set; }

    public virtual ICollection<BattleStat> BattleStats { get; set; } = new List<BattleStat>();

    public virtual CharacterInventory? CharacterInventoryCharacterNNavigation { get; set; }

    public virtual Character? CharacterNameNavigation { get; set; }

    public virtual CityInventory? City { get; set; }

    public virtual Item Item { get; set; } = null!;

    public virtual Npc? Npc { get; set; }

    public virtual Price? Price { get; set; }

    public virtual TradeMarket? TradeMarket { get; set; }
}
