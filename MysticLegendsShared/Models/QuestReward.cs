using System;
using System.Collections.Generic;

namespace MysticLegendsShared.Models;

public partial class QuestReward
{
    public int QuestId { get; set; }

    public int CurrencyGold { get; set; }

    public virtual Quest Quest { get; set; } = null!;
}
