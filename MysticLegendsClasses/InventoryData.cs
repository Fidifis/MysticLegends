using System.Collections.Immutable;

namespace MysticLegendsClasses
{
    public struct InventoryData
    {
        public uint Capacity { get; set; }
        public uint MaxCapacity { get; set; }
        public ImmutableList<ItemData>? Items { get; set; }
    }
}
