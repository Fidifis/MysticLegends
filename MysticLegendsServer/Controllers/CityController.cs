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

    [HttpGet("{city}/storage")]
    public async Task<ObjectResult> GetStorage(string city, string characterName)
    {
        if (!await auth.ValidateAsync(Request.Headers, characterName))
            return StatusCode(403, "Unauthorized");

        var invitems = await dbContext.CityInventories
            .Where(citem => citem.CharacterName == characterName && citem.CityName == city)
            .Include(citem => citem.InventoryItems)
                .ThenInclude(invitem => invitem.Item)
            .Include(citem => citem.InventoryItems)
                .ThenInclude(invitem => invitem.BattleStats)
            .SingleOrDefaultAsync();

        invitems ??= await CreateCityInventory(characterName, city);

        return Ok(invitems);
    }
}
