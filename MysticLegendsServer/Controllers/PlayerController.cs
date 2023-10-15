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
        public CharacterData Get(string username, string characterName, string sessionToken)
        {
            return new CharacterData
            {
                OwnersAccount = username,
                CharacterName = characterName,
                CharacterClass = CharacterClass.Warrior,
                CurrencyGold = 100,
                EquipedItems = ImmutableList.Create<ItemData>(),
                Inventory = new InventoryData
                {
                    Capacity = 5,
                    MaxCapacity = 10,
                    Items = new List<ItemData>()
                    {
                        new()
                        {
                            Name = "Drsnej armor",
                            Icon = "armor_coolBody",
                            ItemType = ItemType.BodyArmor,
                            MaxStack = 1,
                            StackCount = 1,
                            BattleStats = new
                            (
                                new BattleStat[] {
                                    new() {
                                        BattleStatMethod = BattleStat.Method.Base,
                                        BattleStatType = BattleStat.Type.Resilience,
                                        Value = 10,
                                    },
                                    new() {
                                        BattleStatMethod = BattleStat.Method.Base,
                                        BattleStatType = BattleStat.Type.Swiftness,
                                        Value = -1,
                                    },
                                    new() {
                                        BattleStatMethod = BattleStat.Method.Base,
                                        BattleStatType = BattleStat.Type.FireResistance,
                                        Value = 1.5,
                                    },
                                }
                            ),
                        },
                    }.ToImmutableList(),
                },
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
