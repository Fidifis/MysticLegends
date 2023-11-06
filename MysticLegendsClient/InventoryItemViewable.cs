using MysticLegendsShared.Models;

namespace MysticLegendsClient;

public partial class InventoryItemViewable : InventoryItem, IViewableItem
{
    public int Id { get => InvitemId; }
    public string Icon { get => Item.Icon; }
    public int StackNumber { get => StackCount; }
}
