using MysticLegendsShared.Models;

namespace MysticLegendsClient.ApiCalls;

internal static class CityCall
{
    public static Task<CityInventory> GetCityStorageAsync(string city)
    {
        var parameters = new Dictionary<string, string>
        {
            ["characterName"] = GameState.Current.CharacterName,
        };
        return GameState.Current.Connection.GetAsync<CityInventory>($"/api/City/{city}/storage", parameters);
    }
}
