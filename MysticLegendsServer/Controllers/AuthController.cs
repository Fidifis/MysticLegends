using Microsoft.AspNetCore.Mvc;

namespace MysticLegendsServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : Controller
    {
        private Auth auth;
        private ILogger<CharacterController> logger;

        public AuthController(ILogger<CharacterController> logger, Auth auth)
        {
            this.logger = logger;
            this.auth = auth;
        }

        [HttpPost("login")]
        public async Task<ObjectResult> Login([FromBody] Dictionary<string, string> paramters)
        {
            var username = paramters["username"];
            var password = paramters["password"];

            var token = await auth.IssueRefreshToken(username, password);

            if (token is null)
                return BadRequest("Login failed");

            return Ok(token);
        }

        [HttpPost("token")]
        public async Task<ObjectResult> GetAccessToken([FromBody] Dictionary<string, string> paramters)
        {
            var refreshToken = paramters["refreshToken"];

            var token = await auth.IssueAccessToken(refreshToken);

            if (token is null)
                return BadRequest("Issuing token failed");

            return Ok(token);
        }
    }
}
