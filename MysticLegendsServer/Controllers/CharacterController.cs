using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MysticLegendsServer.Models;
using MysticLegendsShared.Models;
using MysticLegendsShared.Utilities;
using System.Collections.Immutable;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MysticLegendsServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CharacterController : ControllerBase
    {
        private readonly Xdigf001Context dbContext;
        private readonly ILogger<CharacterController> logger;
        private readonly Auth auth;
        private readonly IRNG rng;

        public CharacterController(Xdigf001Context context, ILogger<CharacterController> logger, Auth auth, IRNG rng)
        {
            dbContext = context;
            this.logger = logger;
            this.auth = auth;
            this.rng = rng;
        }

        private async Task<Character> RequestCharacterItems(string characterName)
        {
            return await dbContext.Characters
                .Include(character => character.CharacterInventory)
                    .ThenInclude(inventory => inventory!.InventoryItems)
                    .ThenInclude(item => item.Item)
                .Include(character => character.CharacterInventory)
                    .ThenInclude(inventory => inventory!.InventoryItems)
                    .ThenInclude(item => item.BattleStats)
                .Include(character => character.InventoryItems)
                    .ThenInclude(item => item.BattleStats)
                .Include(character => character.InventoryItems)
                    .ThenInclude(item => item.Item)
                .SingleAsync(character => character.CharacterName == characterName);
        }

        // GET api/<PlayerController>/5
        [HttpGet("{characterName}")]
        public async Task<ObjectResult> GetCharacter(string characterName)
        {
            if (!await auth.ValidateAsync(Request.Headers, characterName))
                return StatusCode(403, "Unauthorized");

            return Ok(await RequestCharacterItems(characterName));
        }

        [HttpGet("{characterName}/currency")]
        public async Task<ObjectResult> GetCharacterCurrency(string characterName)
        {
            if (!await auth.ValidateAsync(Request.Headers, characterName))
                return StatusCode(403, "Unauthorized");

            return Ok(await dbContext.Characters
                .Where(character => character.CharacterName == characterName)
                .Select(character => character.CurrencyGold).SingleAsync());
        }

        // POST api/<PlayerController>
        [HttpPost("{characterName}/inventory-swap")]
        public async Task<ObjectResult> InventorySwap(string characterName, [FromBody] Dictionary<string, string> paramters)
        {
            if (!await auth.ValidateAsync(Request.Headers, characterName))
                return StatusCode(403, "Unauthorized");

            var itemToMove = int.Parse(paramters["itemId"]);
            var targetPosition = int.Parse(paramters["position"]);
            var inventory = await dbContext.CharacterInventories
                .Include(inventory => inventory.InventoryItems)
                    .ThenInclude(item => item.Item)
                .Include(inventory => inventory.InventoryItems)
                    .ThenInclude(item => item.BattleStats)
                .SingleAsync(inv => inv.CharacterName == characterName);


            var itemList = inventory.InventoryItems;

            var sourceItem = itemList.SingleOrDefault(item => item.InvitemId == itemToMove);
            var targetItem = itemList.SingleOrDefault(item => item.Position == targetPosition);

            if (sourceItem is null)
            {
                var msg = "swaping empty positions";
                logger.LogWarning(msg);
                return BadRequest(msg);
            }

            var sourcePosition = sourceItem.Position;
            sourceItem.Position = targetPosition;


            if (targetItem is not null)
            {
                targetItem.Position = sourcePosition;
            }

            await dbContext.SaveChangesAsync();

            return Ok(inventory);
        }

        [HttpPost("{characterName}/equip-item")]
        public async Task<ObjectResult> EquipItem(string characterName, [FromBody] Dictionary<string, string> paramters)
        {
            if (!await auth.ValidateAsync(Request.Headers, characterName))
                return StatusCode(403, "Unauthorized");

            var character = await RequestCharacterItems(characterName);
            var inventoryItems = character.CharacterInventory!.InventoryItems;
            var equipedItems = character.InventoryItems;

            var itemToEquipId = int.Parse(paramters["equipItemId"]);
            var itemToEquip = inventoryItems.SingleOrDefault(item => item.InvitemId == itemToEquipId);

            if (itemToEquip is null)
                return BadRequest("didn't find the requested item");

            if (!((ItemType)itemToEquip.Item.ItemType).IsEquipable())
            {
                var msg = "this item cannot be equiped";
                logger.LogWarning(msg);
                return BadRequest(msg);
            }

            if (equipedItems.SingleOrDefault(item => item.Item.ItemType == itemToEquip.Item.ItemType) is not null)
            {
                var msg = "trying to equip already equiped item type";
                logger.LogWarning(msg);
                return BadRequest(msg);
            }

            equipedItems.Add(itemToEquip);
            inventoryItems.Remove(itemToEquip);

            await dbContext.SaveChangesAsync();
            return Ok(character);
        }

        [HttpPost("{characterName}/unequip-item")]
        public async Task<ObjectResult> UnequipItem(string characterName, [FromBody] Dictionary<string, string> paramters)
        {
            if (!await auth.ValidateAsync(Request.Headers, characterName))
                return StatusCode(403, "Unauthorized");

            var character = await RequestCharacterItems(characterName);
            var inventoryItems = character.CharacterInventory!.InventoryItems;
            var equipedItems = character.InventoryItems;

            var itemToUnequipId = int.Parse(paramters["unequipItemId"]);
            var itemToUnequip = equipedItems.SingleOrDefault(item => item.InvitemId == itemToUnequipId);

            var strposition = paramters.Get("position");

            if (itemToUnequip is null)
            {
                var msg = "didn't find the requested item";
                logger.LogWarning(msg);
                return BadRequest(msg);
            }

            var desiredPosition = strposition is not null ? int.Parse(strposition) : itemToUnequip.Position;

            var newPosition = InventoryHandling.FindPositionInInventory(character.CharacterInventory, desiredPosition);

            if (newPosition is null)
            {
                var msg = "No space in inventory";
                logger.LogWarning(msg);
                return BadRequest(msg);
            }

            itemToUnequip.Position = (int)newPosition;

            inventoryItems.Add(itemToUnequip);
            equipedItems.Remove(itemToUnequip);

            await dbContext.SaveChangesAsync();
            return Ok(character);
        }

        [HttpPost("{characterName}/swap-equip-item")]
        public async Task<ObjectResult> SwapEquipItem(string characterName, [FromBody] Dictionary<string, string> paramters)
        {
            if (!await auth.ValidateAsync(Request.Headers, characterName))
                return StatusCode(403, "Unauthorized");

            var character = await RequestCharacterItems(characterName);
            var inventoryItems = character.CharacterInventory!.InventoryItems;
            var equipedItems = character.InventoryItems;

            var itemToEquipId = int.Parse(paramters["equipItemId"]);
            var itemToEquip = inventoryItems.SingleOrDefault(item => item.InvitemId == itemToEquipId);

            if (itemToEquip is null)
            {
                var msg = "didn't find the requested item";
                logger.LogWarning(msg);
                return BadRequest(msg);
            }

            var itemToUnequip = equipedItems.SingleOrDefault(item => item.Item.ItemType == itemToEquip.Item.ItemType);

            if (itemToUnequip is null)
            {
                var msg = "didn't find the right item to be unequiped";
                logger.LogWarning(msg);
                return BadRequest(msg);
            }

            if (!((ItemType)itemToEquip.Item.ItemType).IsEquipable())
            {
                var msg = "this item cannot be equiped";
                logger.LogWarning(msg);
                return BadRequest(msg);
            }

            itemToUnequip.Position = itemToEquip.Position;

            equipedItems.Add(itemToEquip);
            inventoryItems.Remove(itemToEquip);
            inventoryItems.Add(itemToUnequip);
            equipedItems.Remove(itemToUnequip);

            await dbContext.SaveChangesAsync();
            return Ok(character);
        }

        [HttpPost("{characterName}/travel")]
        public async Task<ObjectResult> Travel(string characterName, [FromBody] Dictionary<string, string> paramters)
        {
            if (!await auth.ValidateAsync(Request.Headers, characterName))
                return StatusCode(403, "Unauthorized");

            const int travelWaitTime = 10;
            var city = paramters["city"];

            var character = await dbContext.Characters
                .Where(character => character.CharacterName == characterName)
                .Include(character => character.Travel)
                .SingleAsync();

            if (character.Travel is not null)
            {
                if (character.Travel.Arrival > Time.Current)
                {
                    var msg = "Cannot travel, Character already traveling";
                    logger.LogWarning(msg);
                    return BadRequest(msg);
                }
                else
                    character.Travel = null;
            }

            character.Travel = new Travel()
            {
                CharacterName = characterName,
                Arrival = Time.Current.AddSeconds(travelWaitTime),
            };
            character.CityName = city;

            await dbContext.SaveChangesAsync();

            return Ok(travelWaitTime);
        }

        [HttpGet("{characterName}/city")]
        public async Task<ObjectResult> GetCity(string characterName)
        {
            if (!await auth.ValidateAsync(Request.Headers, characterName))
                return StatusCode(403, "Unauthorized");

            var character = await dbContext.Characters
                .Where(character => character.CharacterName == characterName)
                .Include(character => character.Travel)
                .SingleAsync();

            var response = new Dictionary<string, string>();

            response["city"] = character.CityName;
            if (character.Travel is not null)
            {
                var diff = (int)(character.Travel.Arrival - Time.Current).TotalSeconds;
                if (diff > 0)
                    response["travel"] = (diff + 1).ToString();
            }

            return Ok(response);
        }

        [HttpPost("{characterName}/fight")]
        public async Task<ObjectResult> FightMethod(string characterName, [FromBody] Dictionary<string, string> paramters)
        {
            if (!await auth.ValidateAsync(Request.Headers, characterName))
                return StatusCode(403, "Unauthorized");

            const int travelWaitTime = 15;

            var mobId = int.Parse(paramters["mobId"]);

            var character = await dbContext.Characters
                .Where(character => character.CharacterName == characterName)
                .Include(character => character.Travel)
                .Include(character => character.CharacterInventory)
                    .ThenInclude(inventory => inventory!.InventoryItems)
                .Include(character => character.InventoryItems)
                    .ThenInclude(invitem => invitem.BattleStats)
                .SingleAsync();

            var mob = await dbContext.Mobs
                .Include(mob => mob.MobItemDrops)
                    .ThenInclude(drop => drop.Item)
                .Include(mob => mob.BattleStats)
                .SingleAsync(mob => mob.MobId == mobId);

            if (character.Travel is not null)
            {
                if (character.Travel.Arrival > Time.Current)
                {
                    var msg = "Cannot travel, Character already traveling";
                    logger.LogWarning(msg);
                    return BadRequest(msg);
                }
                else
                    character.Travel = null;
            }

            character.Travel = new Travel()
            {
                CharacterName = characterName,
                Arrival = Time.Current.AddSeconds(travelWaitTime),
                AreaName = mob.AreaName,
            };

            var battleStatsArray = from item in character.InventoryItems where item.BattleStats is not null select new BattleStats(item.BattleStats);
            var battleStats = new BattleStats(battleStatsArray);

            var mobBattleStats = new BattleStats(mob.BattleStats);

            var battleResult = Fight.DecideFight(rng, battleStats, mobBattleStats);

            if (!battleResult)
            {
                await dbContext.SaveChangesAsync(); // save travel info
                return Ok(new BattleResponse
                {
                    Win = false,
                    TravelTime = travelWaitTime,
                    DropedItems = Array.Empty<InventoryItem>(),
                    ReturnCity = character.CityName,
                });
            }

            var drops = new List<InventoryItem>();

            var orderedMobItemDrops = mob.MobItemDrops.OrderBy(drop => drop.DropRate);

            MobItemDrop? lastItem = null;
            foreach (var possibleDrop in orderedMobItemDrops)
            {
                lastItem = possibleDrop;
                var random = rng.RandomDecimal(1.0);
                if (possibleDrop.DropRate < random)
                {
                    continue;
                }

                var added = AddDropedItem(rng, possibleDrop, character, ref drops);

                if (!added)
                    break;
            }

            if (drops.Count == 0 && lastItem is not null)
                AddDropedItem(rng, lastItem, character, ref drops);

            var xp = character.Xp;
            var level = character.Level;

            xp += Convert.ToInt32(Math.Pow(mob.Level, 2));
            Leveling.LevelUpIfPossible(ref level, ref xp);

            character.Xp = xp;
            character.Level = level;

            await dbContext.SaveChangesAsync();
            return Ok(new BattleResponse
            {
                Win = true,
                TravelTime = travelWaitTime,
                DropedItems = drops,
                ReturnCity = character.CityName,
            });
        }

        private static bool AddDropedItem(IRNG rng, MobItemDrop possibleDrop, Character character, ref List<InventoryItem> drops)
        {
            var stack = rng.RandomNumber(1, possibleDrop.MaxAmount);
            var invitem = ItemGenerator.MakeInventoryItem(rng, possibleDrop.Item, possibleDrop.Mob.Level, character.CharacterName, stack);
            var newPosition = InventoryHandling.FindPositionInInventory(character.CharacterInventory!);

            if (newPosition is null)
            {
                return false;
            }
            invitem.Position = newPosition.Value;

            drops.Add(invitem);
            character.CharacterInventory!.InventoryItems.Add(invitem);

            return true;
        }
    }
}
