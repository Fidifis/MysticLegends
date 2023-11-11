using MysticLegendsShared.Models;
using MysticLegendsShared.Utilities;

namespace MysticLegendsClient.ApiCalls;

internal static class UserCall
{
    public static async Task<List<Character>> GetUserCharactersServerCallAsync(string username) => await GameState.Current.Connection.GetAsync<List<Character>>($"/api/User/{username}/characters");

    public static async Task CreateCharacter(string username, string characterName, CharacterClass characterClass)
    {
        var parameters = new Dictionary<string, string>
        {
            ["characterName"] = characterName,
            ["characterClass"] = characterClass.ToString(),
        };
        await ErrorCatcher.TryAsync(async () =>
        {
            await GameState.Current.Connection.PostAsync<string>($"/api/User/{username}/create-character", parameters);
        });
    }
}
