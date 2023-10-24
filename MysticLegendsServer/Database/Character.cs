using MysticLegendsClasses;

namespace MysticLegendsServer.Database
{
    public static class Character
    {
        public enum InventorySource
        {
            CharacterEquiped,
            CharacterInventory,
        }

        private static string GetInventoryOwnerString(InventorySource owner)
        {
            return owner switch
            {
                InventorySource.CharacterEquiped => "character_name",
                InventorySource.CharacterInventory => "character_inventory_character_n",
                _ => throw new Exception()
            };
        }

        public async static Task<CharacterData?> GetCharacterData(string characterName)
        {
            var data = await DB.Connection!.Query($"SELECT username, character_class, currency_gold FROM CHARACTER WHERE CHARACTER_NAME = '{characterName}'");
            if (data.Count == 0)
                return null;
            if (data.Count > 1)
            { /* TODO: Log warning */ }

            var equipedItems = await GetInventoryItems(InventorySource.CharacterEquiped, characterName).ConstructImmutableList();

            var record = data[0];
            var characterData = new CharacterData
            {
                CharacterName = characterName,
                OwnersAccount = record[0],
                CharacterClass = (CharacterClass)int.Parse(record[1]),
                CurrencyGold = int.Parse(record[2]),
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

            var inventoryItems = await GetInventoryItems(InventorySource.CharacterInventory, characterName).ConstructImmutableList();

            var record = data[0];
            var inventoryData = new InventoryData
            {
                Capacity = int.Parse(record[0]),
                Items = inventoryItems,
            };

            return inventoryData;
        }

        public static async IAsyncEnumerable<ItemData> GetInventoryItems(InventorySource inventorySource, string characterName)
        {
            var inventoryAttribute = GetInventoryOwnerString(inventorySource);
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
                        InvItemId = invItemId,
                        ItemId = reader.GetInt32(1),
                        Name = reader.GetString(2),
                        Icon = reader.GetString(3),
                        ItemType = (ItemType)reader.GetInt32(4),
                        Level = reader.GetInt32(5),
                        StackCount = reader.GetInt32(6),
                        MaxStack = reader.GetInt32(7),
                        StackMeansDurability = reader.GetBoolean(8),
                        InventoryPosition = reader.GetInt32(9),
                    },
                    new Dictionary<BattleStat.Type, BattleStat>());

                if (!reader.IsDBNull(10))
                {
                    var battleStatType = (BattleStat.Type)reader.GetInt32(10);
                    itemBattleStats[invItemId].Item2[battleStatType] = new BattleStat
                    {
                        BattleStatType = battleStatType,
                        BattleStatMethod = (BattleStat.Method)reader.GetInt32(11),
                        Value = reader.GetDouble(12),
                    };
                }
            }

            foreach (var item in itemBattleStats)
            {
                var newItem = item.Value.Item1;
                newItem.BattleStats = new(item.Value.Item2);
                yield return newItem;
            }
        }

        public static async Task ChangeItemPosition(int invItemId, int newPosition)
        {
            var sql = $"""
                UPDATE INVENTORY_ITEM
                SET "position" = {newPosition}
                WHERE INVITEM_ID = {invItemId}
                """;

            await DB.Connection!.NonQuery(sql);
        }

        public static async Task ItemTransfer(int invItemId, InventorySource fromInventorySource, InventorySource targetInventorySource, string key)
        {
            var sql = $"""
                UPDATE INVENTORY_ITEM
                SET "{GetInventoryOwnerString(fromInventorySource)}" = NULL,
                "{GetInventoryOwnerString(targetInventorySource)}" = '{key}'
                WHERE INVITEM_ID = {invItemId}
                """;

            await DB.Connection!.NonQuery(sql);
        }
    }
}
