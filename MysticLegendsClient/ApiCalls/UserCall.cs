using MysticLegendsShared.Models;
using MysticLegendsShared.Utilities;

namespace MysticLegendsClient.ApiCalls;

internal static class UserCall
{
    public static Task<List<Character>> GetUserCharactersServerCallAsync(string username) => GameState.Current.Connection.GetAsync<List<Character>>($"/api/User/{username}/characters");

    public static Task<string> CreateCharacter(string username, string characterName, CharacterClass characterClass)
    {
        var parameters = new Dictionary<string, string>
        {
            ["characterName"] = characterName,
            ["characterClass"] = ((int)characterClass).ToString(),
        };

        return GameState.Current.Connection.PostAsync<string>($"/api/User/{username}/create-character", parameters);

    }
}
