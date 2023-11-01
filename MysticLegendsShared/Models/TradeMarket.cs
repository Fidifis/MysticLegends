using System;
using System.Collections.Generic;

namespace MysticLegendsShared.Models;

public partial class TradeMarket
{
    public int InvitemId { get; set; }

    public int PriceGold { get; set; }

    public virtual InventoryItem Invitem { get; set; } = null!;
}
