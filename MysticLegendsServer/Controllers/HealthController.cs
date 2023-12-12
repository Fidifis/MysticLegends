using Microsoft.AspNetCore.Mvc;
using MysticLegendsServer.Models;

namespace MysticLegendsServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HealthController : Controller
    {
        private Xdigf001Context dbContext;
        public HealthController(Xdigf001Context context)
        {
            dbContext = context;
        }

        [HttpGet]
        public async Task<Dictionary<string, string>> Get()
        {
            var dbStatus = await dbContext.Database.CanConnectAsync();
            return new Dictionary<string, string>()
            {
                ["status"] = dbStatus ? "ok" : "database fail"
            };
        }
    }
}
