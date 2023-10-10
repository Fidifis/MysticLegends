using Microsoft.AspNetCore.Mvc;
using MysticLegendsClasses;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MysticLegendsServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlayerController : ControllerBase
    {
        // GET: api/<PlayerController>
        [HttpGet]
        public CharacterData Get()
        {
            return new CharacterData
            {
                OwnersAccountId = 1,
                CharacterId = 1,
                CharacterClass = CharacterClass.Hunter,
                Inventory = new InventoryData
                {
                    Capacity = 5,
                    MaxCapacity = 10,
                    Items = new List<ItemData>()
                },
                CurrencyGold = 100,
            };
        }

        // GET api/<PlayerController>/5
        [HttpGet("{id}")]
        public CharacterData Get(int id)
        {
            return new CharacterData
            {
                OwnersAccountId = (uint)id,
                CharacterId = (uint)id,
                CharacterClass = CharacterClass.Hunter,
                Inventory = new InventoryData
                {
                    Capacity = 5,
                    MaxCapacity = 10,
                    Items = new List<ItemData>()
                },
                CurrencyGold = 100,
            };
        }

        // POST api/<PlayerController>
        [HttpPost]
        public string Post([FromBody] string value)
        {
            return value;
        }

        // PUT api/<PlayerController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<PlayerController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
