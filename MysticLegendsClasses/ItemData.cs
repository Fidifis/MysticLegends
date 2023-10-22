namespace MysticLegendsClasses
{
    public struct ItemData
    {
        public int ItemId { get; set; }
        public int InvItemId { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public ItemType ItemType { get; set; }
        public BattleStats? BattleStats { get; set; }
        public int Level { get; set; }
        public int InventoryPosition { get; set; }
        public bool StackMeansDurability { get; set; }
        public int StackCount { get; set; }
        public int MaxStack { get; set; }
    }
}
