using MysticLegendsShared.Utilities;
using MysticLegendsShared.Models;

namespace MysticLegendsShared.Utilities
{
    [Obsolete]
    public interface IInventory
    {
        public int Capacity { get; set; }
        public ICollection<InventoryItem> InventoryItems { get; set; }
    }

    /// <summary>
    /// The class is used for the need to create an "artificial" inventory that does not match any inventory type from Database Models.
    /// For example, for the need to insert a custom item list into controls that require the <see cref="IInventory"/> interface.
    /// </summary>
    [Obsolete]
    public class ArtifficialInventory: IInventory
    {
        public int Capacity { get; set; } = -1;
        public virtual ICollection<InventoryItem> InventoryItems { get; set; } = new List<InventoryItem>();
    }
}

namespace MysticLegendsShared.Models
{
    public partial class CharacterInventory : IInventory { }
    public partial class CityInventory : IInventory { }
}
