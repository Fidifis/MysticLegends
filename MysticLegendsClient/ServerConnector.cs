using MysticLegendsShared.Models;

namespace MysticLegendsClient;

class ServerConnector
{
    public enum ServerConncetionType
    {
        OfficialServers,
        Localhost,
        Custom
    }

    public static string ConnectionTypeToUrl(ServerConncetionType connectionType, string? customSubstitute = null)
    {
        return connectionType switch
        {
            ServerConncetionType.OfficialServers => GameState.OfficialServersUrl,
            ServerConncetionType.Localhost => "http://localhost:5281",
            ServerConncetionType.Custom => customSubstitute ?? "",
            _ => ""
        };
    }

    public static async Task<bool> Authenticate(GameState gameState)
    {
        var refreshToken = await gameState.TokenStore.ReadRefreshTokenAsync(gameState.Connection.Host);
        if (refreshToken is null)
            return false;

        try
        {
            var accessToken = await ApiCalls.AuthCall.TokenServerCallAsync(refreshToken, gameState);
            gameState.ChangeAccessToken(accessToken);
            gameState.Username = await gameState.TokenStore.ReadUserNameAsync(gameState.Connection.Host) ?? "";
            return true;
        }
        catch (Exception) { }

        return false;
    }

    public static async Task Login(string username, string password, bool saveToken, GameState gameState)
    {
        var refreshToken = await ApiCalls.AuthCall.LoginServerCallAsync(username, password, gameState);
        var accessToken = await ApiCalls.AuthCall.TokenServerCallAsync(refreshToken, gameState);

        if (saveToken == true)
        {
            await gameState.TokenStore.SaveRefreshToken(refreshToken, gameState.Connection.Host);
            await gameState.TokenStore.SaveUsername(username, gameState.Connection.Host);
        }

        gameState.ChangeAccessToken(accessToken);
        gameState.Username = username;
    }

    public static async Task Register(string username, string password, bool saveToken, GameState gameState)
    {
        var refreshToken = await ApiCalls.AuthCall.RegisterServerCallAsync(username, password, gameState);
        var accessToken = await ApiCalls.AuthCall.TokenServerCallAsync(refreshToken, gameState);

        if (saveToken == true)
        {
            await gameState.TokenStore.SaveRefreshToken(refreshToken, gameState.Connection.Host);
            await gameState.TokenStore.SaveUsername(username, gameState.Connection.Host);
        }

        gameState.ChangeAccessToken(accessToken);
        gameState.Username = username;
    }

    public static async Task Logout(GameState gameState)
    {
        var refreshToken = await gameState.TokenStore.ReadRefreshTokenAsync(gameState.Connection.Host);
        var accessToken = gameState.TokenStore.AccessToken;

            await ApiCalls.AuthCall.LogoutServerCallAsync(refreshToken, accessToken);
            gameState.ChangeAccessToken(null);
            await gameState.TokenStore.SaveRefreshToken(null, gameState.Connection.Host);
        
    }
}
