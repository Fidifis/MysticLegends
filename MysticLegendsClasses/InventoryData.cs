namespace MysticLegendsClasses
{
    public struct InventoryData
    {
        public uint Capacity { get; set; }
        public uint MaxCapacity { get; set; }
        public List<ItemData> Items { get; set; }
    }
}
