using System;
using System.Collections.Generic;

namespace MysticLegendsShared.Models;

public partial class Item
{
    public int ItemId { get; set; }

    public string Name { get; set; } = null!;

    public string Icon { get; set; } = null!;

    public int ItemType { get; set; }

    public int MaxStack { get; set; }

    public int? MaxDurability { get; set; }

    public virtual ICollection<InventoryItem> InventoryItems { get; set; } = new List<InventoryItem>();

    public virtual ICollection<MobItemDrop> MobItemDrops { get; set; } = new List<MobItemDrop>();

    public virtual ICollection<QuestRequirement> QuestRequirements { get; set; } = new List<QuestRequirement>();
}
