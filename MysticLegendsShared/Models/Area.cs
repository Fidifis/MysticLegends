using System;
using System.Collections.Generic;

namespace MysticLegendsShared.Models;

public partial class Area
{
    public string AreaName { get; set; } = null!;

    public virtual ICollection<Mob> Mobs { get; set; } = new List<Mob>();

    public virtual ICollection<Travel> Travels { get; set; } = new List<Travel>();
}
