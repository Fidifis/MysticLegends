using Microsoft.AspNetCore.Mvc;
using MysticLegendsClasses;

namespace MysticLegendsServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HealthController : Controller
    {
        [HttpGet]
        public Dictionary<string, string> Get()
        {
            return new()
            {
                ["status"] = "ok"
            };
        }
    }
}
