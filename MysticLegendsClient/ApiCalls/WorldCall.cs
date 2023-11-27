using MysticLegendsShared.Models;

namespace MysticLegendsClient.ApiCalls;

internal static class WorldCall
{
    // Lets try array instead of list
    public static Task<City[]> GetCitiesAsync() => GameState.Current.Connection.GetAsync<City[]>("/api/World/cities");
}
