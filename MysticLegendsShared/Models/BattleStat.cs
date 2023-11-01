using System;
using System.Collections.Generic;

namespace MysticLegendsShared.Models;

public partial class BattleStat
{
    public int StatType { get; set; }

    public int Method { get; set; }

    public int InvitemId { get; set; }

    public double Value { get; set; }

    public virtual InventoryItem Invitem { get; set; } = null!;
}
