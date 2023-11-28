using MysticLegendsShared.Models;
using MysticLegendsShared.Utilities;

namespace MysticLegendsClient.ApiCalls;

internal static class CharacterCall
{
    // TODO: Review API Calls
    // Do i want return void and event invoke or return task<result>
    // do it unified accross all api calls

    public static Task<Character> GetCharacterServerCallAsync(string characterName) => GameState.Current.Connection.GetAsync<Character>($"/api/Character/{characterName}");

    public static async void UpdateCharacter(object? sender, string characterName)
    {
        await ErrorCatcher.TryAsync(async () =>
        {
            var result = await GetCharacterServerCallAsync(characterName);
            GameState.Current.GameEvents.CharacterWithItemsUpdate(sender, new(result));
        });
    }

    public static Task<Dictionary<string, string>> GetCharacterCityCallAsync(string characterName) => 
        GameState.Current.Connection.GetAsync<Dictionary<string, string>>($"/api/Character/{characterName}/city");

    public static Task<int> GetCharacterCurrencyCallAsync(string characterName) => GameState.Current.Connection.GetAsync<int>($"/api/Character/{characterName}/currency");

    public static async void UpdateCurrency(object? sender, string characterName)
    {
        await ErrorCatcher.TryAsync(async () =>
        {
            var currency = await GetCharacterCurrencyCallAsync(characterName);
            GameState.Current.GameEvents.CurrencyUpdate(sender, new(currency));
        });
    }

    public static async void SwapServerCall(object? sender, int itemId, int position)
    {
        var parameters1 = new Dictionary<string, string>
        {
            ["itemId"] = itemId.ToString(),
            ["position"] = position.ToString(),
        };
        await ErrorCatcher.TryAsync(async () =>
        {
            var newInventory1 = await GameState.Current.Connection.PostAsync<CharacterInventory>($"/api/Character/{GameState.Current.CharacterName}/inventory-swap", parameters1);
            GameState.Current.GameEvents.CharacterInventoryUpdate(sender, new(newInventory1.InventoryItems.AsReadOnly()));
        });
    }

    public static async void EquipServerCall(object? sender, int itemToEquip)
    {
        var parameters = new Dictionary<string, string>
        {
            ["equipItemId"] = itemToEquip.ToString(),
        };
        await ErrorCatcher.TryAsync(async() =>
        {
            var characterData = await GameState.Current.Connection.PostAsync<Character>($"/api/Character/{GameState.Current.CharacterName}/equip-item", parameters);
            GameState.Current.GameEvents.CharacterWithItemsUpdate(sender, new(characterData));
        });
    }

    public static async void UnequipServerCall(object? sender, int itemToUnequip, int? position)
    {
        var parameters = new Dictionary<string, string>
        {
            ["unequipItemId"] = itemToUnequip.ToString(),
        };
        if (position is not null)
            parameters["position"] = position.ToString()!;
        await ErrorCatcher.TryAsync(async () =>
        {
            var characterData = await GameState.Current.Connection.PostAsync<Character>($"/api/Character/{GameState.Current.CharacterName}/unequip-item", parameters);
            GameState.Current.GameEvents.CharacterWithItemsUpdate(sender, new(characterData));
        });
    }

    public static async void EquipSwapServerCall(object? sender, int itemToSwapEquip)
    {
        var parameters = new Dictionary<string, string>
        {
            ["equipItemId"] = itemToSwapEquip.ToString(),
        };
        await ErrorCatcher.TryAsync(async () =>
        {
            var characterData = await GameState.Current.Connection.PostAsync<Character>($"/api/Character/{GameState.Current.CharacterName}/swap-equip-item", parameters);
            GameState.Current.GameEvents.CharacterWithItemsUpdate(sender, new(characterData));
        });
    }

    public static Task<int> TravelToCity(string city)
    {
        var parameters = new Dictionary<string, string>
        {
            ["city"] = city,
        };
        return GameState.Current.Connection.PostAsync<int>($"/api/Character/{GameState.Current.CharacterName}/travel", parameters);
    }
}
