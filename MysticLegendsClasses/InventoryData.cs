using System.Collections.Immutable;

namespace MysticLegendsClasses
{
    public struct InventoryData
    {
        public int Capacity { get; set; }
        public int MaxCapacity { get; set; }
        public ImmutableList<ItemData>? Items { get; set; }
    }
}
