namespace MysticLegendsClasses
{
    public struct ItemData
    {
        public int ItemId { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public ItemType ItemType { get; set; }
        public BattleStats? BattleStats { get; set; }
        public uint InventoryPosition { get; set; }
        public bool StackMeansDurability { get; set; }
        public uint StackCount { get; set; }
        public uint MaxStack { get; set; }
    }
}
