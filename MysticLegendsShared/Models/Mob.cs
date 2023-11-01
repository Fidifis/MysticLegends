using System;
using System.Collections.Generic;

namespace MysticLegendsShared.Models;

public partial class Mob
{
    public int MobId { get; set; }

    public string Name { get; set; } = null!;

    public int Level { get; set; }

    public virtual ICollection<MobItemDrop> MobItemDrops { get; set; } = new List<MobItemDrop>();

    public virtual ICollection<QuestRequirement> QuestRequirements { get; set; } = new List<QuestRequirement>();
}
