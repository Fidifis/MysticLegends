using System;
using System.Collections.Generic;

namespace MysticLegendsShared.Models;

public partial class QuestRequirement
{
    public int RequirementId { get; set; }

    public int QuestId { get; set; }

    public int? ItemId { get; set; }

    public int Amount { get; set; }

    public int? MobType { get; set; }

    public virtual Item? Item { get; set; }

    public virtual Quest Quest { get; set; } = null!;
}
