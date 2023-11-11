using Microsoft.AspNetCore.Mvc;
using MysticLegendsServer.Models;

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
    }
}
