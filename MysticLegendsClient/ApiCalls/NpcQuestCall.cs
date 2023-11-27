using MysticLegendsShared.Models;

namespace MysticLegendsClient.ApiCalls;

internal static class NpcQuestCall
{
    public static Task<List<Quest>> GetOfferedQuestsServerCallAsync(int npcId, string characterName)
    {
        var parameters = new Dictionary<string, string>
        {
            ["characterName"] = characterName,
        };
        return GameState.Current.Connection.GetAsync<List<Quest>>($"api/NpcQuest/{npcId}/offered-quests", parameters);
    }

    public static Task<bool> GetQuestCompletableCallAsync(int questId, string characterName)
    {
        var parameters = new Dictionary<string, string>
        {
            ["questId"] = questId.ToString(),
        };
        return GameState.Current.Connection.GetAsync<bool>($"api/NpcQuest/{characterName}/quest-completable", parameters);
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

    public static async Task CompleteQuestServerCallAsync(object? sender, string characterName, int questId)
    {
        var parameters = new Dictionary<string, string>
        {
            ["questId"] = questId.ToString(),
        };
        var response = await GameState.Current.Connection.PostAsync<Character>($"api/NpcQuest/{characterName}/complete-quest", parameters);
        GameState.Current.GameEvents.CharacterUpdate(sender, new(response));
    }
}
