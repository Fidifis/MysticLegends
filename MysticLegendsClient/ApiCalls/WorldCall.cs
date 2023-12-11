using MysticLegendsShared.Models;

namespace MysticLegendsClient.ApiCalls;

internal static class WorldCall
{
    // Lets try array instead of list
    public static Task<City[]> GetCitiesAsync() => GameState.Current.Connection.GetAsync<City[]>("/api/World/cities");

    public static Task<Area[]> GetAreasAsync() => GameState.Current.Connection.GetAsync<Area[]>("/api/World/areas");

    public static Task<Npc[]> GetNpcsInCity(string city) => GameState.Current.Connection.GetAsync<Npc[]>($"/api/World/{city}/npcs");

    public static Task<Mob[]> GetMobsInArea(string area) => GameState.Current.Connection.GetAsync<Mob[]>($"/api/World/{area}/mobs");
}
