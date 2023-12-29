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

    public static Task<CityInventory> SwapStorageItemAsync(string city, int invitemId, int position)
    {
        var parameters = new Dictionary<string, string>
        {
            ["characterName"] = GameState.Current.CharacterName,
            ["itemId"] = invitemId.ToString(),
            ["position"] = position.ToString(),
        };
        return GameState.Current.Connection.PostAsync<CityInventory>($"/api/City/{city}/storage/swap", parameters);
    }

    public static Task<CityInventory> StoreItemAsync(string city, int invitemId, int position)
    {
        var parameters = new Dictionary<string, string>
        {
            ["characterName"] = GameState.Current.CharacterName,
            ["itemId"] = invitemId.ToString(),
            ["position"] = position.ToString(),
        };
        return GameState.Current.Connection.PostAsync<CityInventory>($"/api/City/{city}/storage/store", parameters);
    }

    public static Task<CityInventory> RetreiveItemAsync(string city, int invitemId, int position)
    {
        var parameters = new Dictionary<string, string>
        {
            ["characterName"] = GameState.Current.CharacterName,
            ["itemId"] = invitemId.ToString(),
            ["position"] = position.ToString(),
        };
        return GameState.Current.Connection.PostAsync<CityInventory>($"/api/City/{city}/storage/retrieve", parameters);
    }
}
