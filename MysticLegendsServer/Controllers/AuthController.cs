using Microsoft.AspNetCore.Mvc;
using MysticLegendsShared.Utilities;

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

        [HttpPost("register")]
        public async Task<ObjectResult> Register([FromBody] Dictionary<string, string> paramters)
        {
            var username = paramters["username"];
            var password = paramters["password"];

            var user = await auth.RegisterUser(username, password);
            if (user is null)
            {
                return BadRequest("Registration failed");
            }

            var token = await auth.IssueRefreshToken(username, password, user);

            if (token is null)
                return BadRequest("Login failed");

            return Ok(token);
        }

        [HttpPost("logout")]
        public async Task<ObjectResult> Logout([FromBody] Dictionary<string, string> paramters)
        {
            var accessToken = paramters.Get("accessToken");
            var refreshToken = paramters.Get("refreshToken");

            bool accessTokenResult = true, refreshTokenResult = true;
            if (accessToken is not null)
                accessTokenResult = await auth.InvalidateAccessToken(accessToken);

            if (refreshToken is not null)
                refreshTokenResult = await auth.InvalidateRefreshToken(refreshToken);

            if (refreshTokenResult && accessTokenResult)
            {
                return Ok("ok");
            }
            else
            {
                var msg = "Invalidation of tokens failed";
                logger.LogWarning(msg);
                return BadRequest(msg);
            }
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

        //[HttpPost("validate")]
        //public async Task<ObjectResult> ValidateAccessToken([FromBody] Dictionary<string, string> paramters)
        //{
        //    var accessToken = paramters["accessToken"];
        //    var username = paramters["username"];

        //    return Ok(await auth.ValidateUserAsync(accessToken, username));
        //}
    }
}
