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
        private Xdigf001Context context;

        public PlayerController(Xdigf001Context context)
        {
            this.context = context;
        }

        public static Character lolool = new Character
        {
            Username = "kokot",
            CharacterName = "zmrdus",
            CharacterClass = (int)CharacterClass.Warrior,
            CurrencyGold = 100,
            CharacterInventory = new CharacterInventory
            {
                Capacity = 10,
                InventoryItems = new List<InventoryItem>() {
                    new InventoryItem
                    {
                        Position = 1,
                        Item = new Item() {
                            Icon = "bodyArmor/ayreimWarrior",
                            ItemType = (int)ItemType.BodyArmor,
                        },
                    },
                    new InventoryItem
                    {
                        Position = 4,
                        Item = new Item() {
                            Icon = "helmet/ayreimWarrior",
                            ItemType = (int)ItemType.Helmet,
                        }
                    },
                    new InventoryItem
                    {
                        Position = 3,
                        Item = new Item() {
                            Icon = "bodyArmor/ayreimWarrior",
                            ItemType = (int)ItemType.BodyArmor,
                        }
                    } },
            },
        };

        // GET api/<PlayerController>/5
        [HttpGet("{characterName}")]
        public async Task<Character> Get(string characterName, string accessToken)
        {
            return await context.Characters
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

        //// POST api/<PlayerController>
        //[HttpPost("{characterName}/inventoryswap")]
        //public async Task<ObjectResult> InventorySwap(string characterName, [FromBody] Dictionary<string, string> paramters)
        //{
        //    var sourcePosition = int.Parse(paramters["sourceItem"]);
        //    var targetPosition = int.Parse(paramters["targetItem"]);
        //    var inventory = (await Character.GetCharacterInventory(characterName)).Value;
        //    var newItems = inventory.Items!.ToList();

        //    var sourceIndex = newItems.FindIndex(item => item.InventoryPosition == sourcePosition);
        //    var targetIndex = newItems.FindIndex(item => item.InventoryPosition == targetPosition);

        //    if (sourceIndex == targetIndex)
        //        return BadRequest("{username}/{characterName}/inventoryswap => swaping empty positions");

        //    //Task? task1 = null, task2 = null;
        //    if (sourceIndex >= 0)
        //    {
        //        var sourceItem = newItems[sourceIndex];
        //        sourceItem.InventoryPosition = targetPosition;
        //        newItems[sourceIndex] = sourceItem;
        //        await Character.ChangeItemPosition(sourceItem.InvItemId, targetPosition);
        //    }

        //    if (targetIndex >= 0)
        //    {
        //        var targetItem = newItems[targetIndex];
        //        targetItem.InventoryPosition = sourcePosition;
        //        newItems[targetIndex] = targetItem;
        //        await Character.ChangeItemPosition(targetItem.InvItemId, sourcePosition);
        //    }

        //    inventory.Items = newItems.ToImmutableList();
        //    //await Task.WhenAll(task1 ?? Task.CompletedTask, task2 ?? Task.CompletedTask);

        //    return Ok(inventory);
        //}

        //[HttpPost("{characterName}/equipitem")]
        //public async Task<ObjectResult> EquipItem(string characterName, [FromBody] Dictionary<string, string> paramters)
        //{
        //    var characterData = (await Character.GetCharacterData(characterName)).Value;
        //    var inventoryItems = characterData.Inventory.Items!.ToList();
        //    var equipedItems = characterData.EquipedItems!.ToList();

        //    if (paramters.ContainsKey("itemToEquip"))
        //    {
        //        var itemToEquipPosition = uint.Parse(paramters["itemToEquip"]);
        //        var itemToEquipIndex = inventoryItems.FindIndex(item => item.InventoryPosition == itemToEquipPosition);

        //        if (itemToEquipIndex >= 0)
        //        {
        //            var itemToEquip = inventoryItems[itemToEquipIndex];
        //            var itemToUnequipIndex = equipedItems.FindIndex(item => item.ItemType == itemToEquip.ItemType);

        //            equipedItems.Add(itemToEquip);
        //            inventoryItems.RemoveAt(itemToEquipIndex);

        //            var transferTask1 = Character.ItemTransfer(itemToEquip.InvItemId,
        //                    Character.InventorySource.CharacterInventory,
        //                    Character.InventorySource.CharacterEquiped,
        //                    characterName);

        //            Task? transferTask2 = null;
        //            if (itemToUnequipIndex >= 0)
        //            {
        //                var itemToUnequip = equipedItems[itemToUnequipIndex];
        //                itemToUnequip.InventoryPosition = itemToEquip.InventoryPosition;

        //                equipedItems.RemoveAt(itemToUnequipIndex);
        //                inventoryItems.Add(itemToUnequip);

        //                transferTask2 = Character.ItemTransfer(itemToUnequip.InvItemId,
        //                    Character.InventorySource.CharacterEquiped,
        //                    Character.InventorySource.CharacterInventory,
        //                    characterName);
        //            }

        //            var newInventory = characterData.Inventory;
        //            newInventory.Items = inventoryItems.ToImmutableList();
        //            characterData.Inventory = newInventory;

        //            characterData.EquipedItems = equipedItems.ToImmutableList();

        //            await Task.WhenAll(transferTask1, transferTask2 ?? Task.CompletedTask);
        //            return Ok(characterData);
        //        }
        //    }
        //    if (paramters.ContainsKey("itemToUnequip"))
        //    {
        //        if (inventoryItems.Count >= characterData.Inventory.Capacity)
        //            return BadRequest("{characterName}/equipitem => inventory full");

        //        var itemToUnequipPosition = uint.Parse(paramters["itemToUnequip"]);
        //        var itemToUnequipIndex = equipedItems.FindIndex(item => item.ItemType == (ItemType)itemToUnequipPosition);

        //        if (itemToUnequipIndex >= 0)
        //        {
        //            var itemToUnequip = equipedItems[itemToUnequipIndex];

        //            int itemNewInventoryPosition = 0;
        //            if (paramters.ContainsKey("itemToEquip"))
        //                itemNewInventoryPosition = int.Parse(paramters["itemToEquip"]);
        //            else
        //                itemNewInventoryPosition = itemToUnequip.InventoryPosition;

        //            for (int i = 0; i < inventoryItems.Count; i++)
        //            {
        //                if (inventoryItems.FindIndex(item => item.InventoryPosition == itemNewInventoryPosition) >= 0)
        //                {
        //                    itemNewInventoryPosition++;
        //                    itemNewInventoryPosition = itemNewInventoryPosition >= inventoryItems.Count ? itemNewInventoryPosition - inventoryItems.Count : itemNewInventoryPosition;
        //                }
        //                else
        //                    break;
        //            }

        //            itemToUnequip.InventoryPosition = itemNewInventoryPosition;
        //            var task1 = Character.ChangeItemPosition(itemToUnequip.InvItemId, itemNewInventoryPosition);

        //            equipedItems.RemoveAt(itemToUnequipIndex);
        //            inventoryItems.Add(itemToUnequip);
        //            var task2 = Character.ItemTransfer(itemToUnequip.InvItemId,
        //                    Character.InventorySource.CharacterEquiped,
        //                    Character.InventorySource.CharacterInventory,
        //                    characterName);

        //            var newInventory = characterData.Inventory;
        //            newInventory.Items = inventoryItems.ToImmutableList();
        //            characterData.Inventory = newInventory;

        //            characterData.EquipedItems = equipedItems.ToImmutableList();

        //            await Task.WhenAll(task1, task2);
        //            return Ok(characterData);
        //        }
        //    }

        //    return BadRequest("something went wrong try again :)");
        //}
    }
}
