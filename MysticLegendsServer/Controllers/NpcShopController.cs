using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MysticLegendsServer.Models;
using System.Text.Json;

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

        private static int EstimateSellPrice(int npcId, IReadOnlyCollection<int> items)
        {
            return items.Count * 10;
        }

        [HttpGet("{npcId}/offered-items")]
        public async Task<ObjectResult> GetOfferedItems(int npcId)
        {
            await dbContext.InventoryItems
                .Where(item => item.NpcId == npcId)
                .Include(invItem => invItem.Price)
                .Where(invItem => invItem.Price != null)
                .Take(100)
                .Include(invItem => invItem.Item)
                .Include(invItem => invItem.BattleStats).LoadAsync();
            return Ok(dbContext.InventoryItems);
        }

        [HttpPost("{npcId}/estimate-sell-price")]
        public ObjectResult EstimateSellPrice(int npcId, [FromBody] Dictionary<string, string> paramters)
        {
            var jsonString = paramters["items"];
            var items = JsonSerializer.Deserialize<List<int>>(jsonString)!;

            return Ok(EstimateSellPrice(npcId, items));
        }

        [HttpPost("{npcId}/sell-items")]
        public async Task<ObjectResult> SellItems(int npcId, [FromBody] Dictionary<string, string> paramters)
        {
            var jsonString = paramters["items"];
            var characterString = paramters["character_name"];

            var items = JsonSerializer.Deserialize<List<int>>(jsonString)!;
            var price = EstimateSellPrice(npcId, items);

            var characterAsync = dbContext.Characters.SingleAsync(character => character.CharacterName == characterString);
            var sellItems = dbContext.InventoryItems.Where(item => items.Contains(item.ItemId));

            foreach(var item in sellItems)
            {
                // TODO: check if the item owner is linked with access token
                if (item.CharacterInventoryCharacterN is null || item.CharacterInventoryCharacterN != characterString)
                {
                    logger.LogWarning("Trying to sell item not owned by the correct character");
                    continue;
                }

                item.CharacterInventoryCharacterN = null;
                item.NpcId = npcId;
            }

            var character = await characterAsync;
            character.CurrencyGold += price;

            await dbContext.SaveChangesAsync();

            return Ok(character.CurrencyGold);
        }
    }
}
