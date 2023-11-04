using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MysticLegendsServer.Models;
using MysticLegendsShared.Models;
using MysticLegendsShared.Utilities;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MysticLegendsServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CharacterController : ControllerBase
    {
        private Xdigf001Context dbContext;
        ILogger<CharacterController> logger;

        public CharacterController(Xdigf001Context context, ILogger<CharacterController> logger)
        {
            dbContext = context;
            this.logger = logger;
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
        public async Task<Character> GetCharacter(string characterName, string accessToken)
        {
            return await RequestCharacterItems(characterName);
        }

        [HttpGet("{characterName}/currency")]
        public async Task<int> GetCharacterCurrency(string characterName, string accessToken)
        {
            return await dbContext.Characters
                .Where(character => character.CharacterName == characterName)
                .Select(character => character.CurrencyGold).FirstAsync();
        }

        // POST api/<PlayerController>
        [HttpPost("{characterName}/inventory-swap")]
        public async Task<ObjectResult> InventorySwap(string characterName, [FromBody] Dictionary<string, string> paramters)
        {
            var itemToMove = int.Parse(paramters["itemId"]);
            var targetPosition = int.Parse(paramters["position"]);
            var accessToken = paramters["accessToken"];
            var inventory = await dbContext.CharacterInventories
                .Include(inventory => inventory.InventoryItems)
                    .ThenInclude(item => item.Item)
                .Include(inventory => inventory.InventoryItems)
                    .ThenInclude(item => item.BattleStats)
                .SingleAsync(inv => inv.CharacterName == characterName);


            var itemList = (List<InventoryItem>)inventory.InventoryItems;

            var sourceIndex = itemList.FindIndex(item => item.InvitemId == itemToMove);
            var targetIndex = itemList.FindIndex(item => item.Position == targetPosition);

            if (sourceIndex < 0)
            {
                var msg = "swaping empty positions";
                logger.LogWarning(msg);
                return BadRequest(msg);
            }

            var sourceItem = inventory.InventoryItems.ElementAt(sourceIndex);
            var sourcePosition = sourceItem.Position;
            sourceItem.Position = targetPosition;


            if (targetIndex >= 0)
            {
                var targetItem = inventory.InventoryItems.ElementAt(targetIndex);
                targetItem.Position = sourcePosition;
            }

            await dbContext.SaveChangesAsync();

            return Ok(inventory);
        }

        [HttpPost("{characterName}/equip-item")]
        public async Task<ObjectResult> EquipItem(string characterName, [FromBody] Dictionary<string, string> paramters)
        {
            var character = await RequestCharacterItems(characterName);
            var inventoryItems = (List<InventoryItem>)character.CharacterInventory!.InventoryItems;
            var equipedItems = (List<InventoryItem>)character.InventoryItems;

            var itemToEquipId = int.Parse(paramters["equipItemId"]);
            var itemToEquipIndex = inventoryItems.FindIndex(item => item.InvitemId == itemToEquipId);

            if (itemToEquipIndex < 0)
                return BadRequest("didn't find the requested item");

            var itemToEquip = inventoryItems[itemToEquipIndex];

            if (equipedItems.Find(item => item.Item.ItemType == itemToEquip.Item.ItemType) is not null)
            {
                var msg = "trying to equip already equiped item type";
                logger.LogWarning(msg);
                return BadRequest(msg);
            }

            equipedItems.Add(itemToEquip);
            inventoryItems.RemoveAt(itemToEquipIndex);

            await dbContext.SaveChangesAsync();
            return Ok(character);
        }

        [HttpPost("{characterName}/unequip-item")]
        public async Task<ObjectResult> UnequipItem(string characterName, [FromBody] Dictionary<string, string> paramters)
        {
            var character = await RequestCharacterItems(characterName);
            var inventoryItems = (List<InventoryItem>)character.CharacterInventory!.InventoryItems;
            var equipedItems = (List<InventoryItem>)character.InventoryItems;

            var itemToUnequipId = int.Parse(paramters["unequipItemId"]);
            var itemToUnequipIndex = equipedItems.FindIndex(item => item.InvitemId == itemToUnequipId);

            var strposition = paramters.Get("position");

            if (itemToUnequipIndex < 0)
            {
                var msg = "didn't find the requested item";
                logger.LogWarning(msg);
                return BadRequest(msg);
            }

            var itemToUnequip = equipedItems[itemToUnequipIndex];
            var itemNewInventoryPosition = strposition is not null ? int.Parse(strposition) : itemToUnequip.Position;
            bool positionFound = false;

            var capacity = character.CharacterInventory!.Capacity;
            for (int i = 0; i < capacity; i++)
            {
                if (inventoryItems.Find(item => item.Position == itemNewInventoryPosition) is not null)
                {
                    if (++itemNewInventoryPosition >= capacity)
                        itemNewInventoryPosition -= capacity;
                }
                else
                {
                    positionFound = true;
                    break;
                }
            }

            if (!positionFound)
            {
                var msg = "No space in inventory";
                logger.LogWarning(msg);
                return BadRequest(msg);
            }

            itemToUnequip.Position = itemNewInventoryPosition;

            inventoryItems.Add(itemToUnequip);
            equipedItems.RemoveAt(itemToUnequipIndex);

            await dbContext.SaveChangesAsync();
            return Ok(character);
        }

        [HttpPost("{characterName}/swap-equip-item")]
        public async Task<ObjectResult> SwapEquipItem(string characterName, [FromBody] Dictionary<string, string> paramters)
        {
            var character = await RequestCharacterItems(characterName);
            var inventoryItems = (List<InventoryItem>)character.CharacterInventory!.InventoryItems;
            var equipedItems = (List<InventoryItem>)character.InventoryItems;

            var itemToEquipId = int.Parse(paramters["equipItemId"]);
            var itemToEquipIndex = inventoryItems.FindIndex(item => item.InvitemId == itemToEquipId);

            if (itemToEquipIndex < 0)
            {
                var msg = "didn't find the requested item";
                logger.LogWarning(msg);
                return BadRequest(msg);
            }

            var itemToUnequipIndex = equipedItems.FindIndex(item => item.Item.ItemType == inventoryItems[itemToEquipIndex].Item.ItemType);

            if (itemToUnequipIndex < 0)
            {
                var msg = "didn't find the right item to be unequiped";
                logger.LogWarning(msg);
                return BadRequest(msg);
            }

            var itemToEquip = inventoryItems[itemToEquipIndex];
            var itemToUnequip = equipedItems[itemToUnequipIndex];

            itemToUnequip.Position = itemToEquip.Position;

            equipedItems.Add(itemToEquip);
            inventoryItems.RemoveAt(itemToEquipIndex);
            inventoryItems.Add(itemToUnequip);
            equipedItems.RemoveAt(itemToUnequipIndex);

            await dbContext.SaveChangesAsync();
            return Ok(character);
        }
    }
}
