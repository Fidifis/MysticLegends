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
    public class PlayerController : ControllerBase
    {
        private Xdigf001Context dbContext;
        ILogger<PlayerController> logger;

        public PlayerController(Xdigf001Context context, ILogger<PlayerController> logger)
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
        public async Task<Character> Get(string characterName, string accessToken)
        {
            logger.LogInformation("get character");
            return await RequestCharacterItems(characterName);
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
                return BadRequest("{username}/{characterName}/inventoryswap => swaping empty positions");

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
                return BadRequest("{characterName}/equip-item => You are trying to equip already equiped item type.");

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
                return BadRequest("didn't find the requested item");

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
                return BadRequest("No space in inventory");

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
                return BadRequest("didn't find the requested item");

            var itemToUnequipIndex = equipedItems.FindIndex(item => item.Item.ItemType == inventoryItems[itemToEquipIndex].Item.ItemType);

            if (itemToUnequipIndex < 0)
                return BadRequest("didn't find the right item to be unequiped");


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
