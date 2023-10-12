using Microsoft.AspNetCore.Mvc;
using MysticLegendsClasses;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MysticLegendsServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlayerController : ControllerBase
    {
        // GET api/<PlayerController>/5
        [HttpGet("{accountId}/{characterId}")]
        public CharacterData Get(uint accountId, uint characterId)
        {
            return new CharacterData
            {
                OwnersAccountId = accountId,
                CharacterId = characterId,
                CharacterClass = CharacterClass.Warrior,
                CurrencyGold = 100,
                EquipedItems = new(),
                Inventory = new InventoryData
                {
                    Capacity = 5,
                    MaxCapacity = 10,
                    Items = new()
                    {
                        new()
                        {
                            ItemId = 0,
                            Name = "Drsnej armor",
                            ItemType = ItemType.BodyArmor,
                            EquipableByCharClass = CharacterClass.Warrior,
                            MaxStack = 1,
                            StackCount = 1,
                            BattleStats = new()
                            {
                                Resilience = 100,
                                FireResistance = 10,
                                Swiftness = -2,
                            }
                        },
                    },
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
