using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MysticLegendsServer.Models;
using MysticLegendsShared.Utilities;

namespace MysticLegendsServer.Controllers
{
    public class TradeController : Controller
    {
        private readonly Xdigf001Context dbContext;
        private readonly ILogger<CharacterController> logger;
        private readonly Auth auth;

        public TradeController(ILogger<CharacterController> logger, Auth auth, Xdigf001Context context)
        {
            dbContext = context;
            this.logger = logger;
            this.auth = auth;
        }

        [HttpGet("list/{page}")]
        public async Task<ObjectResult> GetItems(int page)
        {
            if (page < 1)
                return BadRequest($"Page {page} is invalid. Must be 1 or greater");

            //if (!await auth.ValidateAsync(Request.Headers, characterName))
            //    return StatusCode(403, "Unauthorized");

            //if (await CharacterController.GetCharacterCityAsync(characterName, dbContext) != city)
            //{
            //    var msg = "character is not in the city";
            //    logger.LogWarning(msg);
            //    return BadRequest(msg);
            //}

            var items = await dbContext.InventoryItems
                .Where(invitem => invitem.TradeMarket != null)
                .Include(invitem => invitem.Item)
                .Include(invitem => invitem.BattleStats)
                .OrderBy(invitem => Math.Abs((invitem.TradeMarket!.BiddingEnds - Time.Current).TotalMinutes))
                .Skip(100 * (page - 1))
                .Take(100)
                .ToArrayAsync();

            return Ok(items);
        }

        [HttpPost("buy")]
        public async Task<ObjectResult> BuyItem([FromBody] Dictionary<string, string> paramters)
        {
            var characterName = paramters["characterName"];
            var itemToBuy = int.Parse(paramters["itemId"]);
            var targetPosition = int.Parse(paramters["position"]);

            if (!await auth.ValidateAsync(Request.Headers, characterName))
                return StatusCode(403, "Unauthorized");

            var item = await dbContext.InventoryItems
                .Where(invitem => invitem.InvitemId == itemToBuy)
                .Include(invitem => invitem.Price)
                .Include(invitem => invitem.TradeMarket)
                .SingleAsync();

            var buyer = await dbContext.Characters.SingleAsync(character => character.CharacterName == characterName);
            var seller = await dbContext.Characters.SingleAsync(character => character.CharacterName == item.CityInventoryCharacterName);

            if (buyer.CurrencyGold < item.Price!.PriceGold)
                return BadRequest("Not enough money");

            buyer.CurrencyGold -= item.Price!.PriceGold;
            seller.CurrencyGold += item.Price!.PriceGold;

            item.Price = null;
            item.TradeMarket = null;
            item.CityInventoryCharacterName = buyer.CharacterName;
            item.CityName = buyer.CityName;

            var storage = await StorageController.GetCityInventoryAsync(buyer.CityName, buyer.CharacterName, dbContext);

            var position = InventoryHandling.FindPositionInInventory(storage, targetPosition);
            if (position is null)
                return BadRequest("City storage is full");

            item.Position = position.Value;

            await dbContext.SaveChangesAsync();

            return Ok(buyer.CurrencyGold);
        }

        [HttpPost("sell")]
        public async Task<ObjectResult> SellItem([FromBody] Dictionary<string, string> paramters)
        {
            var characterName = paramters["characterName"];
            var itemToSell = int.Parse(paramters["itemId"]);
            var price = int.Parse(paramters["price"]);

            if (!await auth.ValidateAsync(Request.Headers, characterName))
                return StatusCode(403, "Unauthorized");

            var item = await dbContext.InventoryItems
                .Where(invitem => invitem.InvitemId == itemToSell)
                .Include(invitem => invitem.Price)
                .Include(invitem => invitem.TradeMarket)
                .SingleAsync();

            if (item.CityInventoryCharacterName != characterName)
                return BadRequest("Item is not in city storage of this character");

            item.Price = new()
            {
                PriceGold = price,
            };
            item.TradeMarket = new()
            {
                ListedSince = Time.Current,
                BiddingEnds = Time.Current.AddDays(3),
            };

            await dbContext.SaveChangesAsync();

            return Ok("ok");
        }
    }
}
