using System;
using System.Collections.Generic;

namespace MysticLegendsShared.Models;

public partial class NpcItem
{
    public int NpcItemId { get; set; }

    public int NpcId { get; set; }

    public int? PriceGold { get; set; }

    public virtual InventoryItem? InventoryItem { get; set; }

    public virtual Npc Npc { get; set; } = null!;
}
