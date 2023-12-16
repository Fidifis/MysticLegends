using MysticLegendsShared.Models;
using System.Text.Json;

namespace MysticLegendsClient.ApiCalls;

internal static class NpcCall
{
    public static Task<InventoryItem[]> GetOfferedItemsServerCallAsync(int npcId)
    {
        var parameters = new Dictionary<string, string>
        {
            ["characterName"] = GameState.Current.CharacterName,
        };
        return GameState.Current.Connection.GetAsync<InventoryItem[]>($"api/NpcShop/{npcId}/offered-items", parameters);
    }

    public static Task<int> GetOfferedPriceServerCallAsync(int npcId, IEnumerable<InventoryItem> items)
    {
        var parameters = new Dictionary<string, string>
        {
            ["characterName"] = GameState.Current.CharacterName,
            ["items"] = JsonSerializer.Serialize(items.Select(item => item.InvitemId))
        };
        return GameState.Current.Connection.PostAsync<int>($"api/NpcShop/{npcId}/estimate-sell-price", parameters);
    }

    public static async Task SellItemsServerCallAsync(object? sender, int npcId, IEnumerable<InventoryItem> items)
    {
        var parameters = new Dictionary<string, string>
        {
            ["items"] = JsonSerializer.Serialize(items.Select(item => item.InvitemId)),
            ["characterName"] = GameState.Current.CharacterName,
        };
        await ErrorCatcher.TryAsync(async () =>
        {
            var response = await GameState.Current.Connection.PostAsync<int>($"api/NpcShop/{npcId}/sell-items", parameters);
            GameState.Current.GameEvents.CurrencyUpdate(sender, new(response));
        });
    }

    public static async Task BuyItemServerCallAsync(object? sender, int npcId, int invitemId, int? position = null)
    {
        var parameters = new Dictionary<string, string>
        {
            ["item"] = invitemId.ToString(),
            ["characterName"] = GameState.Current.CharacterName,
        };
        if (position is not null)
            parameters["position"] = position.ToString()!;
        await ErrorCatcher.TryAsync(async () =>
        {
            var response = await GameState.Current.Connection.PostAsync<int>($"api/NpcShop/{npcId}/buy-item", parameters);
            GameState.Current.GameEvents.CurrencyUpdate(sender, new(response));
        });
    }
}
