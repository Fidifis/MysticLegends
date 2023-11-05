using System;
using System.Collections.Generic;

namespace MysticLegendsShared.Models;

// TODO remove this from database and reimport model
[Obsolete("Removed from DB schema")]
public partial class NpcInventory
{
    public string NpcName { get; set; } = null!;

    public string CityName { get; set; } = null!;

    public int Capacity { get; set; }
}
