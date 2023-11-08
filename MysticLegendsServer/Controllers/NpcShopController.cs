using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MysticLegendsServer.Models;

namespace MysticLegendsServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NpcShopController : Controller
    {
        private Xdigf001Context dbContext;
        ILogger<CharacterController> logger;

        public NpcShopController(Xdigf001Context context, ILogger<CharacterController> logger)
        {
            dbContext = context;
            this.logger = logger;
        }

        [HttpGet("{npcId}/offered-items")]
        public ObjectResult GetOfferedItems(int npcId)
        {
            var items = dbContext.InventoryItems
                .Where(item => item.NpcId == npcId)
                .Include(invItem => invItem.Price)
                .Where(invItem => invItem.Price != null)
                .Take(100)
                .Include(invItem => invItem.Item)
                .Include(invItem => invItem.BattleStats);
            return Ok(items);
        }
    }
}
