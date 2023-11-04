using System;
using System.Collections.Generic;

namespace MysticLegendsShared.Models;

public partial class NpcItem
{
    public string NpcName { get; set; } = null!;

    public int InvitemId { get; set; }

    public int? PriceGold { get; set; }

    public virtual InventoryItem Invitem { get; set; } = null!;

    public virtual Npc NpcNameNavigation { get; set; } = null!;
}
