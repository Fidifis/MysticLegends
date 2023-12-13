using MysticLegendsShared.Models;

namespace MysticLegendsShared.Utilities;

public readonly struct BattleResponse
{
    public bool Win { get; init; }
    public int TravelTime { get; init; }
    public IReadOnlyCollection<InventoryItem>? DropedItems { get; init; }
    public string ReturnCity { get; init; }
}
