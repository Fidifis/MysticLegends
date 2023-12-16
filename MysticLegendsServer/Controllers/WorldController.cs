using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MysticLegendsServer.Models;

namespace MysticLegendsServer.Controllers;

[Route("api/[controller]")]
[ApiController]
public class WorldController : Controller
{
    private Xdigf001Context dbContext;
    private ILogger<CharacterController> logger;

    public WorldController(ILogger<CharacterController> logger, Xdigf001Context context)
    {
        dbContext = context;
        this.logger = logger;
    }

    [HttpGet("cities")]
    public async Task<ObjectResult> GetCities() => Ok(await dbContext.Cities.OrderBy(city => city.CityName).ToListAsync());

    [HttpGet("areas")]
    public async Task<ObjectResult> GetAreas() => Ok(await dbContext.Areas.OrderBy(area => area.AreaName).ToListAsync());

    [HttpGet("{city}/npcs")]
    public async Task<ObjectResult> GetNpcsInCity(string city) =>
        Ok(
            await dbContext.Npcs
            .Where(npc => npc.CityName == city)
            .OrderBy(npc => npc.NpcId)
            .ToListAsync()
        );

    [HttpGet("{area}/mobs")]
    public async Task<ObjectResult> GetMobsInArea(string area) =>
        Ok(
            await dbContext.Mobs
            .Where(mob => mob.AreaName == area)
            .Include(mob => mob.MobItemDrops)
                .ThenInclude(drops => drops.Item)
            .OrderBy(mob => mob.Type)
            .ThenBy(mob => mob.Level)
            .ToListAsync()
        );
}
