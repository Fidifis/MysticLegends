using System;
using System.Collections.Generic;

namespace MysticLegendsShared.Models;

public partial class Npc
{
    public int NpcId { get; set; }

    public string CityName { get; set; } = null!;

    public int NpcType { get; set; }

    public int? CurrencyGold { get; set; }

    public virtual City CityNameNavigation { get; set; } = null!;

    public virtual ICollection<InventoryItem> InventoryItems { get; set; } = new List<InventoryItem>();

    public virtual ICollection<Quest> Quests { get; set; } = new List<Quest>();
}
