using MysticLegendsShared.Models;

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

    public static async Task LogoutServerCallAsync(string? refreshToken, string? accessToken, GameState? gameState = null)
    {
        var parameters = new Dictionary<string, string>();
        if (refreshToken is not null)
            parameters["refreshToken"] = refreshToken;
        if (accessToken is not null)
            parameters["accessToken"] = accessToken;
        
        await ErrorCatcher.TryAsync(async () =>
        {
            await (gameState ?? GameState.Current).Connection.PostAsync<string>($"api/Auth/logout", parameters);
        });
    }

    public static async Task<string> TokenServerCallAsync(string refreshToken, GameState? gameState = null)
    {
        var parameters = new Dictionary<string, string>
        {
            ["refreshToken"] = refreshToken,
        };
        return await (gameState ?? GameState.Current).Connection.PostAsync<string>($"api/Auth/token", parameters);
    }

    //public static async Task<bool> ValidateServerCallAsync(string accessToken, string username, GameState? gameState = null)
    //{
    //    var parameters = new Dictionary<string, string>
    //    {
    //        ["accessToken"] = accessToken,
    //        ["username"] = username,
    //    };
    //    return await (gameState ?? GameState.Current).Connection.PostAsync<bool>($"api/Auth/validate", parameters);
    //}
}
