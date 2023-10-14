namespace MysticLegendsClasses
{
    public struct ItemData
    {
        public uint ItemId { get; set; }
        public string Name { get; set; }
        public ItemType ItemType { get; set; }
        public CharacterClass? EquipableByCharClass { get; set; }
        public BattleStats? BattleStats { get; set; }
        public uint StackCount { get; set; }
        public uint MaxStack { get; set; }
    }
}
