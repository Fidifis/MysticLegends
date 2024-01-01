using MysticLegendsShared.Models;

namespace MysticLegendsClient.ApiCalls;

internal static class StorageCall
{
    public static Task<CityInventory> GetCityStorageAsync(string city)
    {
        var parameters = new Dictionary<string, string>
        {
            ["characterName"] = GameState.Current.CharacterName,
        };
        return GameState.Current.Connection.GetAsync<CityInventory>($"/api/Storage/{city}/list", parameters);
    }

    public static Task<CityInventory> SwapStorageItemAsync(string city, int invitemId, int position)
    {
        var parameters = new Dictionary<string, string>
        {
            ["characterName"] = GameState.Current.CharacterName,
            ["itemId"] = invitemId.ToString(),
            ["position"] = position.ToString(),
        };
        return GameState.Current.Connection.PostAsync<CityInventory>($"/api/Storage/{city}/swap", parameters);
    }

    public static Task<CityInventory> StoreItemAsync(string city, int invitemId, int position)
    {
        var parameters = new Dictionary<string, string>
        {
            ["characterName"] = GameState.Current.CharacterName,
            ["itemId"] = invitemId.ToString(),
            ["position"] = position.ToString(),
        };
        return GameState.Current.Connection.PostAsync<CityInventory>($"/api/Storage/{city}/store", parameters);
    }

    public static Task<CharacterInventory> RetreiveItemAsync(string city, int invitemId, int position)
    {
        var parameters = new Dictionary<string, string>
        {
            ["characterName"] = GameState.Current.CharacterName,
            ["itemId"] = invitemId.ToString(),
            ["position"] = position.ToString(),
        };
        return GameState.Current.Connection.PostAsync<CharacterInventory>($"/api/Storage/{city}/retrieve", parameters);
    }
}
