using MysticLegendsShared.Models;
using System.Text.Json;

namespace MysticLegendsClient.ApiCalls;

internal static class NpcQuestCall
{
    public static async Task<List<Quest>> GetOfferedQuestsServerCallAsync(int npcId)
    {
        var parameters = new Dictionary<string, string>
        {
            ["characterName"] = GameState.Current.CharacterName,
        };
        return await GameState.Current.Connection.GetAsync<List<Quest>>($"api/NpcQuest/{npcId}/offered-quests", parameters);
    }

    public static async Task AcceptQuestServerCallAsync(string characterName, int questId)
    {
        var parameters = new Dictionary<string, string>
        {
            ["questId"] = questId.ToString(),
        };
        await GameState.Current.Connection.PostAsync<AcceptedQuest>($"api/NpcQuest/{characterName}/accept-quest", parameters);
    }

    public static async Task AbandonQuestServerCallAsync(string characterName, int questId)
    {
        var parameters = new Dictionary<string, string>
        {
            ["questId"] = questId.ToString(),
        };
        await GameState.Current.Connection.PostAsync<AcceptedQuest>($"api/NpcQuest/{characterName}/abandon-quest", parameters);
    }
}
