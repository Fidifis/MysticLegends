using MysticLegendsShared.Utilities;
using MysticLegendsShared.Models;

namespace MysticLegendsShared.Utilities
{
    public interface IInventory
    {
        public int Capacity { get; set; }
        public ICollection<InventoryItem> InventoryItems { get; set; }
    }
}

namespace MysticLegendsShared.Models
{
    public partial class CharacterInventory : IInventory { }
    public partial class CityInventory : IInventory { }
    public partial class NpcInventory : IInventory { }
}
