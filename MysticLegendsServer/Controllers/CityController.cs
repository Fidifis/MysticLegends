using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MysticLegendsServer.Models;
using MysticLegendsShared.Models;

namespace MysticLegendsServer.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CityController : Controller
{
    private readonly Xdigf001Context dbContext;
    private readonly ILogger<CharacterController> logger;
    private readonly Auth auth;

    public CityController(ILogger<CharacterController> logger, Auth auth, Xdigf001Context context)
    {
        dbContext = context;
        this.logger = logger;
        this.auth = auth;
    }

    private async Task<CityInventory> CreateCityInventory(string characterName, string cityName)
    {
        var cinv = new CityInventory {
            CityName = cityName,
            CharacterName = characterName,
            Capacity = 100,
        };

        dbContext.CityInventories.Add(cinv);
        await dbContext.SaveChangesAsync();

        return cinv;
    }

    private Task<string> GetCharacterCityAsync(string characterName) =>
        dbContext.Characters
        .Where(character => character.CharacterName == characterName)
        .Select(character => character.CityName)
        .SingleAsync();

    private async Task<CityInventory> GetCityInventoryAsync(string city, string characterName)
    {
        var invitems = await dbContext.CityInventories
             .Where(citem => citem.CharacterName == characterName && citem.CityName == city)
             .Include(citem => citem.InventoryItems)
                 .ThenInclude(invitem => invitem.Item)
             .Include(citem => citem.InventoryItems)
                 .ThenInclude(invitem => invitem.BattleStats)
             .SingleOrDefaultAsync();

        invitems ??= await CreateCityInventory(characterName, city);
        return invitems;
    }

    [HttpGet("{city}/storage")]
    public async Task<ObjectResult> GetStorage(string city, string characterName)
    {
        if (!await auth.ValidateAsync(Request.Headers, characterName))
            return StatusCode(403, "Unauthorized");

        if (await GetCharacterCityAsync(characterName) != city)
        {
            var msg = "character is not in the city";
            logger.LogWarning(msg);
            return BadRequest(msg);
        }

        return Ok(await GetCityInventoryAsync(city, characterName));
    }

    [HttpPost("{city}/storage/swap")]
    public async Task<ObjectResult> SwapStorageItem(string city, [FromBody] Dictionary<string, string> paramters)
    {
        var characterName = paramters["characterName"];
        var itemToMove = int.Parse(paramters["itemId"]);
        var targetPosition = int.Parse(paramters["position"]);

        if (!await auth.ValidateAsync(Request.Headers, characterName))
            return StatusCode(403, "Unauthorized");

        if (await GetCharacterCityAsync(characterName) != city)
        {
            var msg = "character is not in the city";
            logger.LogWarning(msg);
            return BadRequest(msg);
        }

        var storage = await GetCityInventoryAsync(city, characterName);

        var itemList = storage.InventoryItems;

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

        return Ok(storage);
    }

    [HttpPost("{city}/storage/store")]
    public async Task<ObjectResult> StoreItem(string city, [FromBody] Dictionary<string, string> paramters)
    {
        var characterName = paramters["characterName"];
        var itemToMove = int.Parse(paramters["itemId"]);
        var targetPosition = int.Parse(paramters["position"]);

        if (!await auth.ValidateAsync(Request.Headers, characterName))
            return StatusCode(403, "Unauthorized");

        if (await GetCharacterCityAsync(characterName) != city)
        {
            var msg = "character is not in the city";
            logger.LogWarning(msg);
            return BadRequest(msg);
        }

        var storage = await GetCityInventoryAsync(city, characterName);

        var sourceItem = await dbContext.InventoryItems
            .Where(invitem => invitem.InvitemId == itemToMove)
            .Include(invitem => invitem.BattleStats)
            .Include(invitem => invitem.Item)
            .SingleAsync();

        if (sourceItem.CharacterInventoryCharacterN != characterName)
        {
            var msg = $"character {characterName} do not have item in inventory";
            logger.LogWarning(msg);
            return BadRequest(msg);
        }

        var position = InventoryHandling.FindPositionInInventory(storage, targetPosition);

        if (position is null)
        {
            var msg = $"cannot store item. storage is full";
            logger.LogWarning(msg);
            return BadRequest(msg);
        }

        sourceItem.Position = position.Value;
        sourceItem.CharacterInventoryCharacterN = null;
        sourceItem.CityInventoryCharacterName = characterName;
        sourceItem.CityName = city;

        storage.InventoryItems.Add(sourceItem);

        await dbContext.SaveChangesAsync();

        return Ok(storage);
    }

    [HttpPost("{city}/storage/retrieve")]
    public async Task<ObjectResult> RetreiveItem(string city, [FromBody] Dictionary<string, string> paramters)
    {
        var characterName = paramters["characterName"];
        var itemToMove = int.Parse(paramters["itemId"]);
        var targetPosition = int.Parse(paramters["position"]);

        if (!await auth.ValidateAsync(Request.Headers, characterName))
            return StatusCode(403, "Unauthorized");

        var sourceItem = await dbContext.InventoryItems
            .Where(invitem => invitem.InvitemId == itemToMove)
            .Include(invitem => invitem.BattleStats)
            .Include(invitem => invitem.Item)
            .SingleAsync();

        if (await GetCharacterCityAsync(characterName) != sourceItem.CityName)
        {
            var msg = "character is not in the city";
            logger.LogWarning(msg);
            return BadRequest(msg);
        }

        var inventoryItems = await dbContext.CharacterInventories
            .Where(inventory => inventory.CharacterName == characterName)
            .Include(inventory => inventory.InventoryItems)
                 .ThenInclude(invitem => invitem.Item)
            .Include(inventory => inventory.InventoryItems)
                 .ThenInclude(invitem => invitem.BattleStats)
            .SingleAsync();

        if (sourceItem.CityInventoryCharacterName != characterName)
        {
            var msg = $"character {characterName} do not have item in inventory";
            logger.LogWarning(msg);
            return BadRequest(msg);
        }

        var position = InventoryHandling.FindPositionInInventory(inventoryItems, targetPosition);

        if (position is null)
        {
            var msg = $"cannot retreive item. inventory is full";
            logger.LogWarning(msg);
            return BadRequest(msg);
        }

        sourceItem.Position = position.Value;
        sourceItem.CharacterInventoryCharacterN = characterName;
        sourceItem.CityInventoryCharacterName = null;
        sourceItem.CityName = null;

        inventoryItems.InventoryItems.Add(sourceItem);

        await dbContext.SaveChangesAsync();

        return Ok(inventoryItems);
    }
}
