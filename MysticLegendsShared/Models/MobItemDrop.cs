using System;
using System.Collections.Generic;

namespace MysticLegendsShared.Models;

public partial class MobItemDrop
{
    public int ItemId { get; set; }

    public int MobId { get; set; }

    public double DropRate { get; set; }

    public int MaxAmount { get; set; }

    public virtual Item Item { get; set; } = null!;

    public virtual Mob Mob { get; set; } = null!;
}
