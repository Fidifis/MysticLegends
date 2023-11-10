namespace MysticLegendsClient.ApiCalls;

internal static class AuthCall
{
    public static async Task<string> LoginServerCallAsync(string username, string password, GameState? gameState = null)
    {
        var parameters = new Dictionary<string, string>
        {
            ["username"] = username,
            ["password"] = password,
        };
        return await (gameState ?? GameState.Current).Connection.PostAsync<string>($"api/Auth/login", parameters);
    }

    public static async Task<string> TokenServerCallAsync(string refreshToken, GameState? gameState = null)
    {
        var parameters = new Dictionary<string, string>
        {
            ["refreshToken"] = refreshToken,
        };
        return await (gameState ?? GameState.Current).Connection.PostAsync<string>($"api/Auth/token", parameters);
    }
}
