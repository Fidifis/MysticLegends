using System;
using System.Collections.Generic;

namespace MysticLegendsShared.Models;

public partial class Mob
{
    public int MobId { get; set; }

    public string AreaName { get; set; } = null!;

    public string MobName { get; set; } = null!;

    public int Type { get; set; }

    public int Level { get; set; }

    public int GroupSize { get; set; }

    public virtual Area AreaNameNavigation { get; set; } = null!;

    public virtual ICollection<MobItemDrop> MobItemDrops { get; set; } = new List<MobItemDrop>();
}
