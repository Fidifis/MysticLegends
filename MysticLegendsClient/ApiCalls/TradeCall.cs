using MysticLegendsShared.Models;

namespace MysticLegendsClient.ApiCalls;

internal class TradeCall
{
    public static Task<InventoryItem[]> GetTradeItemsAsync(int page)
    {
        return GameState.Current.Connection.GetAsync<InventoryItem[]>($"/api/Trade/list/{page}");
    }

    public static Task<int> BuyItemAsync(int invitemId, int position)
    {
        var parameters = new Dictionary<string, string>
        {
            ["characterName"] = GameState.Current.CharacterName,
            ["itemId"] = invitemId.ToString(),
            ["position"] = position.ToString(),
        };
        return GameState.Current.Connection.PostAsync<int>("/api/Trade/buy", parameters);
    }

    public static Task SellItemAsync(int invitemId, int price)
    {
        var parameters = new Dictionary<string, string>
        {
            ["characterName"] = GameState.Current.CharacterName,
            ["itemId"] = invitemId.ToString(),
            ["price"] = price.ToString(),
        };
        return GameState.Current.Connection.PostAsync<string>("/api/Trade/sell", parameters);
    }
}
