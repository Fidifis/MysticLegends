using Microsoft.AspNetCore.Mvc;
using MysticLegendsClasses;
using System.Collections.Immutable;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MysticLegendsServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlayerController : ControllerBase
    {
        public static CharacterData lolool = new CharacterData
        {
            OwnersAccount = "kokot",
            CharacterName = "zmrdus",
            CharacterClass = CharacterClass.Warrior,
            CurrencyGold = 100,
            Inventory = new InventoryData
            {
                Capacity = 10,
                Items = ImmutableList.Create(
                        new ItemData
                        {
                            InventoryPosition = 1,
                            Icon = "bodyArmor/ayreimWarrior",
                            ItemType = ItemType.BodyArmor,
                        },
                        new ItemData
                        {
                            InventoryPosition = 4,
                            Icon = "helmet/ayreimWarrior",
                            ItemType = ItemType.Helmet,
                        },
                        new ItemData
                        {
                            InventoryPosition = 3,
                            Icon = "bodyArmor/ayreimWarrior",
                            ItemType = ItemType.BodyArmor,
                        }),
            },
            EquipedItems = new List<ItemData>()
                    {
                        new()
                        {
                            Name = "Helmet of Ayreim warriors",
                            Icon = "helmet/ayreimWarrior",
                            ItemType = ItemType.Helmet,
                            StackMeansDurability = true,
                            MaxStack = 100,
                            StackCount = 90,
                            BattleStats = new
                            (
                                new BattleStat[] {
                                    new() {
                                        BattleStatType = BattleStat.Type.Resilience,
                                        BattleStatMethod = BattleStat.Method.Multiply,
                                        Value = 1.05,
                                    },
                                }
                            ),
                        },
                        new()
                        {
                            Name = "Armor of Ayreim warriors",
                            Icon = "bodyArmor/ayreimWarrior",
                            ItemType = ItemType.BodyArmor,
                            StackMeansDurability = true,
                            MaxStack = 100,
                            StackCount = 90,
                            BattleStats = new
                            (
                                new BattleStat[] {
                                    new() {
                                        BattleStatType = BattleStat.Type.Resilience,
                                        BattleStatMethod = BattleStat.Method.Add,
                                        Value = 10,
                                    },
                                    new() {
                                        BattleStatType = BattleStat.Type.Swiftness,
                                        BattleStatMethod = BattleStat.Method.Add,
                                        Value = -1,
                                    },
                                    new() {
                                        BattleStatType = BattleStat.Type.FireResistance,
                                        BattleStatMethod = BattleStat.Method.Add,
                                        Value = 1.5,
                                    },
                                }
                            ),
                        },
                    }.ToImmutableList(),
        };

        // GET api/<PlayerController>/5
        [HttpGet("{username}/{characterName}")]
        public async Task<CharacterData> Get(string username, string characterName, string accessToken)
        {
            List<List<string>> data = await DB.Connection!.Query("select * from character");
            
            return lolool;
        }

        // POST api/<PlayerController>
        [HttpPost("{username}/{characterName}/inventoryswap")]
        public ObjectResult InventorySwap(string username, string characterName, [FromBody] Dictionary<string, string> paramters)
        {
            var sourcePosition = uint.Parse(paramters["sourceItem"]);
            var targetPosition = uint.Parse(paramters["targetItem"]);
            var newItems = lolool.Inventory.Items!.ToList();

            var sourceIndex = newItems.FindIndex(item => item.InventoryPosition == sourcePosition);
            var targetIndex = newItems.FindIndex(item => item.InventoryPosition == targetPosition);

            if (sourceIndex == targetIndex)
                return BadRequest("{username}/{characterName}/inventoryswap => swaping empty positions");

            if (sourceIndex >= 0)
            {
                var sourceItem = newItems[sourceIndex];
                sourceItem.InventoryPosition = targetPosition;
                newItems[sourceIndex] = sourceItem;
            }

            if (targetIndex >= 0)
            {
                var targetItem = newItems[targetIndex];
                targetItem.InventoryPosition = sourcePosition;
                newItems[targetIndex] = targetItem;
            }

            var newInv = lolool.Inventory;
            newInv.Items = newItems.ToImmutableList();
            lolool.Inventory = newInv;

            return Ok(lolool.Inventory);
        }

        [HttpPost("{username}/{characterName}/equipitem")]
        public ObjectResult EquipItem(string username, string characterName, [FromBody] Dictionary<string, string> paramters)
        {
            var inventoryItems = lolool.Inventory.Items!.ToList();
            var equipedItems = lolool.EquipedItems!.ToList();

            if (paramters.ContainsKey("itemToEquip"))
            {
                var itemToEquipPosition = uint.Parse(paramters["itemToEquip"]);
                var itemToEquipIndex = inventoryItems.FindIndex(item => item.InventoryPosition == itemToEquipPosition);

                if (itemToEquipIndex >= 0)
                {
                    var itemToEquip = inventoryItems[itemToEquipIndex];
                    var itemToUnequipIndex = equipedItems.FindIndex(item => item.ItemType == itemToEquip.ItemType);

                    equipedItems.Add(itemToEquip);
                    inventoryItems.RemoveAt(itemToEquipIndex);

                    if (itemToUnequipIndex >= 0)
                    {
                        var itemToUnequip = equipedItems[itemToUnequipIndex];
                        itemToUnequip.InventoryPosition = itemToEquip.InventoryPosition;

                        equipedItems.RemoveAt(itemToUnequipIndex);
                        inventoryItems.Add(itemToUnequip);
                    }

                    var newInventory = lolool.Inventory;
                    newInventory.Items = inventoryItems.ToImmutableList();
                    lolool.Inventory = newInventory;

                    lolool.EquipedItems = equipedItems.ToImmutableList();

                    return Ok(lolool);
                }
            }
            if (paramters.ContainsKey("itemToUnequip"))
            {
                if (inventoryItems.Count >= lolool.Inventory.Capacity)
                    return BadRequest("{username}/{characterName}/equipitem => inventory full");

                var itemToUnequipPosition = uint.Parse(paramters["itemToUnequip"]);
                var itemToUnequipIndex = equipedItems.FindIndex(item => item.ItemType == (ItemType)itemToUnequipPosition);

                if (itemToUnequipIndex >= 0)
                {
                    var itemToUnequip = equipedItems[itemToUnequipIndex];

                    uint itemNewInventoryPosition = 0;
                    if (paramters.ContainsKey("itemToEquip"))
                        itemNewInventoryPosition = uint.Parse(paramters["itemToEquip"]);
                    else
                        itemNewInventoryPosition = itemToUnequip.InventoryPosition;

                    for (int i = 0; i < inventoryItems.Count; i++)
                    {
                        if (inventoryItems.FindIndex(item => item.InventoryPosition == itemNewInventoryPosition) >= 0)
                        {
                            itemNewInventoryPosition++;
                            itemNewInventoryPosition = itemNewInventoryPosition >= (uint)inventoryItems.Count ? itemNewInventoryPosition - (uint)inventoryItems.Count : itemNewInventoryPosition;
                        }
                        else
                            break;
                    }

                    itemToUnequip.InventoryPosition = itemNewInventoryPosition;

                    equipedItems.RemoveAt(itemToUnequipIndex);
                    inventoryItems.Add(itemToUnequip);

                    var newInventory = lolool.Inventory;
                    newInventory.Items = inventoryItems.ToImmutableList();
                    lolool.Inventory = newInventory;

                    lolool.EquipedItems = equipedItems.ToImmutableList();

                    return Ok(lolool);
                }
            }

            return BadRequest("something went wrong try again :)");
        }
    }
}
