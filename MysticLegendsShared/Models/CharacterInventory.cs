using System;
using System.Collections.Generic;

namespace MysticLegendsShared.Models;

public partial class CharacterInventory
{
    public string CharacterName { get; set; } = null!;

    public int Capacity { get; set; }

    public virtual Character CharacterNameNavigation { get; set; } = null!;

    public virtual ICollection<InventoryItem> InventoryItems { get; set; } = new List<InventoryItem>();
}
