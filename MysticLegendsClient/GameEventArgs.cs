using MysticLegendsShared.Models;

namespace MysticLegendsClient;

internal class CurrencyUpdateEventArgs: EventArgs
{
    public int Value { get; init; }

    public CurrencyUpdateEventArgs(int value)
    {
        Value = value;
    }
}

internal class CharacterInventoryUpdateEventArgs : EventArgs
{
    public ICollection<InventoryItem> InventoryItems { get; init; }

    public CharacterInventoryUpdateEventArgs(ICollection<InventoryItem> value)
    {
        InventoryItems = value;
    }
}
