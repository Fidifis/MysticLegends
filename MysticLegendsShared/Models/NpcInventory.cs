using System;
using System.Collections.Generic;

namespace MysticLegendsShared.Models;

public partial class NpcInventory
{
    public string NpcName { get; set; } = null!;

    public string CityName { get; set; } = null!;

    public int Capacity { get; set; }
}
