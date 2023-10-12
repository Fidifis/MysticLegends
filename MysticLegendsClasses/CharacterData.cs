namespace MysticLegendsClasses
{
    public struct CharacterData
    {
        public ulong OwnersAccountId { get; set; }
        public ulong CharacterId { get; set; }
        public CharacterClass CharacterClass { get; set; }
        public uint CurrencyGold { get; set; }
        public InventoryData Inventory { get; set; }
        public List<ItemData> EquipedItems { get; set; }
    }
}
