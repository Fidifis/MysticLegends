using MysticLegendsShared.Models;
using MysticLegendsShared.Utilities;

namespace MysticLegendsServer;

public static class ItemGenerator
{
    private const int StatFactor = 10;
    public static IEnumerable<CBattleStat> MakeBattleStats(IRNG rng, ItemType itemType, int itemLevel, int itemId)
    {
        switch (itemType)
        {
            case ItemType.Potion:
                return new CBattleStat[] { MakePotionStats(rng, itemLevel, itemId) };

            default:
                return MakeGeneralStats(rng, itemLevel);
        }
    }

    private static IEnumerable<CBattleStat> MakeGeneralStats(IRNG rng, int itemLevel)
    {
        var numberOfStats = rng.RandomNumber(1, 6);
        return Enumerable.Range(0, numberOfStats).Select(_ => MakeStat(rng, itemLevel));
    }

    private static CBattleStat MakeStat(IRNG rng, int itemLevel)
    {
        CBattleStat.Method method = (CBattleStat.Method)rng.RandomNumber(3);

        var types = Enum.GetValues<CBattleStat.Type>();
        CBattleStat.Type type = types[rng.RandomNumber(types.Length)];

        double value = method switch
        {
            CBattleStat.Method.Add => rng.RandomDecimal(Math.Sqrt(itemLevel), itemLevel * StatFactor),
            CBattleStat.Method.Multiply => rng.RandomDecimal(0.8, Math.Sqrt(itemLevel)),
            CBattleStat.Method.Fix => rng.RandomDecimal(itemLevel * 2, itemLevel < StatFactor * 2 ? itemLevel * StatFactor : (itemLevel * itemLevel / 2)),
            _ => 0.0
        };

        return new CBattleStat(method, type, value);
    }

    private static CBattleStat MakePotionStats(IRNG rng, int itemLevel, int itemId)
    {
        CBattleStat.Type type;
        int size;
        switch (itemId)
        {
            case 5:
                type = CBattleStat.Type.Resilience;
                size = 1;
                break;
            case 6:
                type = CBattleStat.Type.Resilience;
                size = 2;
                break;
            case 7:
                type = CBattleStat.Type.Strength;
                size = 1;
                break;
            case 8:
                type = CBattleStat.Type.Dexterity;
                size = 1;
                break;
            default:
                throw new NotImplementedException("undefined type of potion");
        }

        var min = Math.Sqrt(itemLevel) / 2;
        var max = Math.Sqrt(itemLevel) * size;
        var value = rng.RandomDecimal(min, max);

        return new CBattleStat(CBattleStat.Method.Multiply, type, value);
    }

    public static InventoryItem MakeInventoryItem(IRNG rng, Item item, int itemLevel, string characterOwner, int stack)
    {
        var invitem = new InventoryItem
        {
            CharacterInventoryCharacterN = characterOwner,
            ItemId = item.ItemId,
            Item = item,
            StackCount = stack,
            Level = itemLevel,
            Durability = item.MaxDurability,
        };

        if (CanHaveBattleStats((ItemType)item.ItemType))
        {
            invitem.BattleStats = MakeBattleStats(rng, (ItemType)item.ItemType, itemLevel, item.ItemId)
                .Select(stat => new BattleStat(stat, invitem)).ToList();
        }

        return invitem;
    }

    private static bool CanHaveBattleStats(ItemType itemType) =>
        itemType == ItemType.Potion ||
        itemType.IsEquipable();
}
