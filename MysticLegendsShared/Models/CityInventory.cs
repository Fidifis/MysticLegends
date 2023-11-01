using System;
using System.Collections.Generic;

namespace MysticLegendsShared.Models;

public partial class CityInventory
{
    public string CityName { get; set; } = null!;

    public string CharacterName { get; set; } = null!;

    public int Capacity { get; set; }

    public virtual Character CharacterNameNavigation { get; set; } = null!;

    public virtual City CityNameNavigation { get; set; } = null!;

    public virtual ICollection<InventoryItem> InventoryItems { get; set; } = new List<InventoryItem>();
}
