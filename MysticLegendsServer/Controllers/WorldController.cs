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
    public async Task<ObjectResult> GetCities() => Ok(await dbContext.Cities.ToListAsync());

    [HttpGet("{city}/npcs")]
    public async Task<ObjectResult> GetNpcsInCity(string city) =>
        Ok(
            await dbContext.Npcs
            .Where(npc => npc.CityName == city)
            .OrderBy(npc => npc.NpcId)
            .ToListAsync()
        );
}
