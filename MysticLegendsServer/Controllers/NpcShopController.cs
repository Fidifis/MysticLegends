﻿using Microsoft.AspNetCore.Mvc;
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
        public ObjectResult GetOfferedItems(string npcId)
        {
            var items = dbContext.NpcItems
                .Where(item => item.NpcName == npcId && item.PriceGold != null)
                .Take(100)
                .Include(npcItem => npcItem.Invitem)
                    .ThenInclude(invItem => invItem.Item)
                .Include(npcItem => npcItem.Invitem)
                    .ThenInclude(invItem => invItem.BattleStats);
            return Ok(items);
        }
    }
}
