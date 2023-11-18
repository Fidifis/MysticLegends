using MysticLegendsShared.Models;

namespace MysticLegendsClient.ApiCalls;

internal static class NpcQuestCall
{
    public static async Task<List<Quest>> GetOfferedQuestsServerCallAsync(int npcId, string characterName)
    {
        var parameters = new Dictionary<string, string>
        {
            ["characterName"] = characterName,
        };
        return await GameState.Current.Connection.GetAsync<List<Quest>>($"api/NpcQuest/{npcId}/offered-quests", parameters);
    }

    public static async Task<bool> GetQuestCompletableCallAsync(int questId, string characterName)
    {
        var parameters = new Dictionary<string, string>
        {
            ["questId"] = questId.ToString(),
        };
        return await GameState.Current.Connection.GetAsync<bool>($"api/NpcQuest/{characterName}/quest-completable", parameters);
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

    public static async Task<bool> CompleteQuestServerCallAsync(string characterName, int questId)
    {
        var parameters = new Dictionary<string, string>
        {
            ["questId"] = questId.ToString(),
        };
        return await GameState.Current.Connection.PostAsync<bool>($"api/NpcQuest/{characterName}/complete-quest", parameters);
    }
}
