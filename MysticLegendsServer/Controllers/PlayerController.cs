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
                            Icon = "bodyArmor/ayreimWarrior"
                        },
                        new ItemData
                        {
                            InventoryPosition = 4,
                            Icon = "helmet/ayreimWarrior"
                        },
                        new ItemData
                        {
                            InventoryPosition = 3,
                            Icon = "bodyArmor/ayreimWarrior"
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
        public CharacterData Get(string username, string characterName, string accessToken)
        {
            return lolool;
        }

        // POST api/<PlayerController>
        [HttpPost("{username}/{characterName}/inventoryswap")]
        public InventoryData Post(string username, string characterName, [FromBody] Dictionary<string, string> paramters)
        {
            var sourcePosition = uint.Parse(paramters["sourceItem"]);
            var targetPosition = uint.Parse(paramters["targetItem"]);
            var newItems = lolool.Inventory.Items!.ToList();

            var sourceIndex = newItems.FindIndex(item => item.InventoryPosition == sourcePosition);
            var targetIndex = newItems.FindIndex(item => item.InventoryPosition == targetPosition);

            if (sourceIndex == targetIndex)
                return lolool.Inventory;

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

            return lolool.Inventory;
        }
    }
}
