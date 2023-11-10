using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MysticLegendsServer.Models;
using MysticLegendsShared.Utilities;
using System.Text.Json;

namespace MysticLegendsServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NpcShopController : Controller
    {
        private Xdigf001Context dbContext;
        private ILogger<CharacterController> logger;
        private Auth auth;

        public NpcShopController(Xdigf001Context context, ILogger<CharacterController> logger, Auth auth)
        {
            dbContext = context;
            this.logger = logger;
            this.auth = auth;
        }

        private static int EstimateSellPrice(int npcId, IReadOnlyCollection<int> items)
        {
            return items.Count * 10;
        }

        [HttpGet("{npcId}/offered-items")]
        public async Task<ObjectResult> GetOfferedItems(int npcId, string characterName)
        {
            if (!await auth.ValidateAsync(Request.Headers, characterName))
                return StatusCode(403, "Unauthorized");

            var reslut = await dbContext.InventoryItems
                .Where(item => item.NpcId == npcId)
                .Include(invItem => invItem.Price)
                .Where(invItem => invItem.Price != null)
                .Take(100)
                .Include(invItem => invItem.Item)
                .Include(invItem => invItem.BattleStats).ToListAsync();
            return Ok(reslut);
        }

        [HttpPost("{npcId}/estimate-sell-price")]
        public async Task<ObjectResult> EstimateSellPrice(int npcId, [FromBody] Dictionary<string, string> paramters)
        {
            var characterName = paramters["characterName"];

            if (!await auth.ValidateAsync(Request.Headers, characterName))
                return StatusCode(403, "Unauthorized");

            var jsonString = paramters["items"];
            var items = JsonSerializer.Deserialize<List<int>>(jsonString)!;

            return Ok(EstimateSellPrice(npcId, items));
        }

        [HttpPost("{npcId}/sell-items")]
        public async Task<ObjectResult> SellItems(int npcId, [FromBody] Dictionary<string, string> paramters)
        {
            var characterString = paramters["characterName"];

            if (!await auth.ValidateAsync(Request.Headers, characterString))
                return StatusCode(403, "Unauthorized");

            var jsonString = paramters["items"];

            var items = JsonSerializer.Deserialize<List<int>>(jsonString)!;
            var price = EstimateSellPrice(npcId, items);

            var character = await dbContext.Characters.SingleAsync(character => character.CharacterName == characterString);
            var sellItems = await dbContext.InventoryItems.Where(item => items.Contains(item.InvitemId)).ToListAsync();

            foreach(var item in sellItems)
            {
                // TODO: check if the item owner is linked with access token
                // TODO: check if the character is in the same city as npc
                if (item.CharacterInventoryCharacterN is null || item.CharacterInventoryCharacterN != characterString)
                {
                    var msg = "Trying to sell item not owned by the correct character";
                    logger.LogWarning(msg);
                    return BadRequest(msg);
                }

                item.CharacterInventoryCharacterN = null;
                item.NpcId = npcId;
            }

            character.CurrencyGold += price;

            await dbContext.SaveChangesAsync();

            return Ok(character.CurrencyGold);
        }

        [HttpPost("{npcId}/buy-item")]
        public async Task<ObjectResult> BuyItem(int npcId, [FromBody] Dictionary<string, string> paramters)
        {
            var characterString = paramters["characterName"];

            if (!await auth.ValidateAsync(Request.Headers, characterString))
                return StatusCode(403, "Unauthorized");

            var invitemId = int.Parse(paramters["item"]);
            var position = paramters.Get("position");

            var itemToBuy = await dbContext.InventoryItems
                .Where(item => item.InvitemId == invitemId)
                .Take(1)
                .Include(item => item.Price)
                .SingleAsync();
            var character = await dbContext.Characters
                .Include(character => character.CharacterInventory)
                .ThenInclude(inventory => inventory!.InventoryItems)
                .SingleAsync(character => character.CharacterName == characterString);


            // TODO: check if the item owner is linked with access token
            // TODO: check if the character is in the same city as npc
            if (itemToBuy.NpcId is null || itemToBuy.NpcId != npcId)
            {
                var msg = "Trying to buy item not owned by the correct npc";
                logger.LogWarning(msg);
                return BadRequest(msg);
            }

            if (itemToBuy.Price is null)
            {
                var msg = "Item is not for sale";
                logger.LogWarning(msg);
                return BadRequest(msg);
            }

            if (itemToBuy.Price.PriceGold > character.CurrencyGold)
            {
                var msg = "Not enough money";
                logger.LogWarning(msg);
                return BadRequest(msg);
            }

            var newPosition = InventoryHandling.FindPositionInInventory(character.CharacterInventory!, int.Parse(position ?? "0"));

            if (newPosition is null)
            {
                return BadRequest("Inventory is full");
            }

            itemToBuy.NpcId = null;
            itemToBuy.CharacterInventoryCharacterN = character.CharacterName;
            itemToBuy.Position = (int)newPosition;

            character.CurrencyGold -= itemToBuy.Price.PriceGold;
            itemToBuy.Price = null;

            await dbContext.SaveChangesAsync();

            return Ok(character.CurrencyGold);
        }
    }
}
