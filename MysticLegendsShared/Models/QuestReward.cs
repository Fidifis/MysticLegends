using System;
using System.Collections.Generic;

namespace MysticLegendsShared.Models;

public partial class QuestReward
{
    public int QuestId { get; set; }

    public int? ItemId { get; set; }

    public int? CurrencyGold { get; set; }

    public int? ItemCount { get; set; }

    public virtual Item? Item { get; set; }

    public virtual Quest Quest { get; set; } = null!;
}
