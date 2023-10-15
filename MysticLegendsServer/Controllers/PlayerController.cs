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
        // GET api/<PlayerController>/5
        [HttpGet("{username}/{characterName}")]
        public CharacterData Get(string username, string characterName, string accessToken)
        {
            return new CharacterData
            {
                OwnersAccount = username,
                CharacterName = characterName,
                CharacterClass = CharacterClass.Warrior,
                CurrencyGold = 100,
                Inventory = new(),
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
        }

        // POST api/<PlayerController>
        [HttpPost]
        public string Post([FromBody] string value)
        {
            return value;
        }
    }
}
