using MysticLegendsClasses;
using System.Collections.Immutable;

namespace MysticLegendsServer.Database
{
    public static class Character
    {
        public async static Task<CharacterData?> GetCharacterData(string characterName)
        {
            var data = await DB.Connection!.Query($"SELECT username, character_class, currency_gold FROM CHARACTER WHERE CHARACTER_NAME = '{characterName}'");
            if (data.Count == 0)
                return null;
            if (data.Count > 1)
            { /* TODO: Log warning */ }

            var itemDataBuilder = ImmutableList.CreateBuilder<ItemData>();
            await foreach (var item in GetInventoryItems("CHARACTER_NAME", characterName))
            {
                itemDataBuilder.Add(item);
            }
            var equipedItems = itemDataBuilder.ToImmutable();

            var record = data[0];
            var characterData = new CharacterData
            {
                CharacterName = characterName,
                OwnersAccount = record[0],
                CharacterClass = (CharacterClass)uint.Parse(record[1]),
                CurrencyGold = uint.Parse(record[2]),
                Inventory = (await GetCharacterInventory(characterName)).Value,
                EquipedItems = equipedItems,
            };
            return characterData;
        }

        public static async Task<InventoryData?> GetCharacterInventory(string characterName)
        {
            var data = await DB.Connection!.Query($"SELECT capacity FROM character_inventory WHERE CHARACTER_NAME = '{characterName}'");
            if (data.Count == 0)
                return null;
            if (data.Count > 1)
            { /* TODO: Log warning */ }

            var itemDataBuilder = ImmutableList.CreateBuilder<ItemData>();
            await foreach (var item in GetInventoryItems("CHARACTER_INVENTORY_CHARACTER_N", characterName))
            {
                itemDataBuilder.Add(item);
            }
            var inventoryItems = itemDataBuilder.ToImmutable();

            var record = data[0];
            var inventoryData = new InventoryData
            {
                Capacity = uint.Parse(record[0]),
                Items = inventoryItems,
            };

            return inventoryData;
        }

        public static async IAsyncEnumerable<ItemData> GetInventoryItems(string inventoryAttribute, string characterName)
        {
            string query = $"""
                SELECT "inventory_item"."invitem_id", "inventory_item"."item_id", "name", "icon", "item_type", "level", "stack_count", "max_stack", "stack_means_durability", "position", "stat_type", "method", "value"
                FROM INVENTORY_ITEM
                INNER JOIN ITEM ON INVENTORY_ITEM.ITEM_ID = ITEM.ITEM_ID
                LEFT JOIN BATTLE_STATS ON INVENTORY_ITEM.INVITEM_ID = BATTLE_STATS.INVITEM_ID
                WHERE {inventoryAttribute} = '{characterName}'
                """;
            Dictionary<int, Tuple<ItemData, Dictionary<BattleStat.Type, BattleStat>>> itemBattleStats = new();

            await using var reader = await DB.Connection!.QueryReader(query);
            while (await reader.ReadAsync())
            {
                var invItemId = reader.GetInt32(0);

                if (!itemBattleStats.ContainsKey(invItemId))
                    itemBattleStats[invItemId] = Tuple.Create(new ItemData
                    {
                        InvItemId = (uint)invItemId,
                        ItemId = (uint)reader.GetInt32(1),
                        Name = reader.GetString(2),
                        Icon = reader.GetString(3),
                        ItemType = (ItemType)reader.GetInt32(4),
                        Level = (uint)reader.GetInt32(5),
                        StackCount = (uint)reader.GetInt32(6),
                        MaxStack = (uint)reader.GetInt32(7),
                        StackMeansDurability = reader.GetBoolean(8),
                        InventoryPosition = (uint)reader.GetInt32(9),
                    },
                    new Dictionary<BattleStat.Type, BattleStat>());

                var battleStatType = (BattleStat.Type)reader.GetInt32(10);
                itemBattleStats[invItemId].Item2[battleStatType] = new BattleStat
                {
                    BattleStatType = battleStatType,
                    BattleStatMethod = (BattleStat.Method)reader.GetInt32(11),
                    Value = reader.GetDouble(12),
                };
            }

            //var itemData = new ItemData[itemBattleStats.Count];
            //int i = 0;
            foreach (var item in itemBattleStats)
            {
                var newItem = item.Value.Item1;
                newItem.BattleStats = new(item.Value.Item2);
                //itemData[i++] = newItem;
                yield return newItem;
            }
        }
    }
}
