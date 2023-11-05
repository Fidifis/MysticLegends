using System;
using System.Collections.Generic;

namespace MysticLegendsShared.Models;

public partial class NpcItem
{
    public int NpcItemId { get; set; }

    public string NpcName { get; set; } = null!;

    public int? PriceGold { get; set; }

    public virtual InventoryItem? InventoryItem { get; set; }

    public virtual Npc NpcNameNavigation { get; set; } = null!;
}
