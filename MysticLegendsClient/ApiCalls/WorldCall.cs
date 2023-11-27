using MysticLegendsShared.Models;

namespace MysticLegendsClient.ApiCalls;

internal static class WorldCall
{
    public static Task<City[]> GetCitiesAsync() => GameState.Current.Connection.GetAsync<City[]>("/api/World/cities");
}
