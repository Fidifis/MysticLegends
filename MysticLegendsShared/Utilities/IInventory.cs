namespace MysticLegendsShared.Models;

public interface IInventory
{
    public int Capacity { get; set; }
    public ICollection<InventoryItem> InventoryItems { get; set; }
}

public partial class CharacterInventory: IInventory { }
public partial class CityInventory : IInventory { }
public partial class NpcInventory : IInventory { }
