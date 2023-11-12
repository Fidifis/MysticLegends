using System;
using System.Collections.Generic;

namespace MysticLegendsShared.Models;

public partial class Price
{
    public int InvitemId { get; set; }

    public int PriceGold { get; set; }

    public int? BidGold { get; set; }

    public int? QuantityPerPurchase { get; set; }

    public virtual InventoryItem Invitem { get; set; } = null!;
}
