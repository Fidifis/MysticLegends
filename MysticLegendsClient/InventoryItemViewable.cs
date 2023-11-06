using MysticLegendsShared.Models;

namespace MysticLegendsClient;

public partial class InventoryItemViewable : IViewableItem
{
    public InventoryItem Source { get; init; }

    public int Id => Source.InvitemId;
    public string Icon => Source.Item.Icon;
    public int StackNumber => Source.StackCount;
    public int Position => Source.Position;

    public InventoryItemViewable(InventoryItem source)
    {
        Source = source;
    }
}
