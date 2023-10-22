using System.Collections.Immutable;

namespace MysticLegendsClasses
{
    public struct CharacterData
    {
        public string OwnersAccount { get; set; }
        public string CharacterName { get; set; }
        public CharacterClass CharacterClass { get; set; }
        public int CurrencyGold { get; set; }
        public InventoryData Inventory { get; set; }
        public ImmutableList<ItemData>? EquipedItems { get; set; }
    }
}
