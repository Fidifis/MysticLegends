﻿using System;
using System.Collections.Generic;

namespace MysticLegendsShared.Models;

public partial class TradeMarket
{
    public int InvitemId { get; set; }

    public DateTime ListedSince { get; set; }

    public DateTime BiddingEnds { get; set; }

    public virtual InventoryItem Invitem { get; set; } = null!;
}
