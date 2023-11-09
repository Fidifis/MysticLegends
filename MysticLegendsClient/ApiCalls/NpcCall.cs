using MysticLegendsShared.Models;
using System.Text.Json;

namespace MysticLegendsClient.ApiCalls;

internal static class NpcCall
{
    public static async Task<List<InventoryItem>> GetOfferedItemsServerCallAsync(int npcId)
    {
        return await GameState.Current.Connection.GetAsync<List<InventoryItem>>($"api/NpcShop/{npcId}/offered-items");
    }

    public static async Task<int> GetOfferedPriceServerCallAsync(int npcId, IReadOnlyCollection<InventoryItem> items)
    {
        var parameters = new Dictionary<string, string>
        {
            ["items"] = JsonSerializer.Serialize(items.Select(item => item.InvitemId))
        };
        return await GameState.Current.Connection.PostAsync<int>($"api/NpcShop/{npcId}/estimate-sell-price", parameters);
    }

    public static async Task SellItemsServerCallAsync(object? sender, int npcId, IReadOnlyCollection<InventoryItem> items)
    {
        var parameters = new Dictionary<string, string>
        {
            ["items"] = JsonSerializer.Serialize(items.Select(item => item.InvitemId)),
            ["character_name"] = "zmrdus",
        };
        var response = await GameState.Current.Connection.PostAsync<int>($"api/NpcShop/{npcId}/sell-items", parameters);
        GameState.Current.GameEvents.CurrencyUpdate(sender, new(response));
    }

    public static async Task BuyItemServerCallAsync(object? sender, int npcId, int invitemId)
    {
        var parameters = new Dictionary<string, string>
        {
            ["item"] = invitemId.ToString(),
            ["character_name"] = "zmrdus",
        };
        var response = await GameState.Current.Connection.PostAsync<int>($"api/NpcShop/{npcId}/buy-item", parameters);
        GameState.Current.GameEvents.CurrencyUpdate(sender, new(response));
    }
}
