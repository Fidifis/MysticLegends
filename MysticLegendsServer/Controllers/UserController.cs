using Microsoft.AspNetCore.Mvc;
using MysticLegendsServer.Models;
using MysticLegendsShared.Models;
using MysticLegendsShared.Utilities;

namespace MysticLegendsServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : Controller
    {
        private Xdigf001Context dbContext;
        private Auth auth;
        private ILogger<CharacterController> logger;

        public UserController(Auth auth, ILogger<CharacterController> logger, Xdigf001Context context)
        {
            dbContext = context;
            this.auth = auth;
            this.logger = logger;
        }

        [HttpGet("{username}/characters")]
        public async Task<ObjectResult> GetCharacters(string username)
        {
            if (!await auth.ValidateUserAsync(Request.Headers, username))
                return StatusCode(403, "Unauthorized");

            var characters = dbContext.Characters.Where(character => character.Username == username).ToList();
            return Ok(characters);
        }

        [HttpPost("{username}/create-character")]
        public async Task<ObjectResult> CreateCharacter(string username, [FromBody] Dictionary<string, string> paramters)
        {
            if (!await auth.ValidateUserAsync(Request.Headers, username))
                return StatusCode(403, "Unauthorized");

            var characterName = paramters["characterName"].Trim();
            var characterClass = int.Parse(paramters["characterClass"]);

            if (characterName == "")
                return BadRequest("Empty character name");
            if (!Enum.IsDefined((CharacterClass)characterClass))
                return BadRequest("Wrong character class");

            var character = new Character()
            {
                CharacterName = characterName,
                CityName = "Ayreim",
                Username = username,
                CharacterClass = characterClass,
                Level = 1,
                CurrencyGold = 1000,
            };

            dbContext.Characters.Add(character);
            await dbContext.SaveChangesAsync();

            return Ok(characterName);
        }
    }
}
