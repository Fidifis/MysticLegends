namespace MysticLegendsClient;

internal class GameState : IDisposable
{
    public static GameState Current { get; private set; } = new();
    public ApiClient Connection { get; private init; }
    public TokenStore TokenStore { get; private init; } = new();
    public ConfigStore ConfigStore { get; private init; } = new();

    public GameEvents GameEvents { get; private init; } = new();

    public const string OfficialServersUrl = "https://servers.mysticlegends.fidifis.com";

    public static void MakeGameStateCurrent(GameState gs)
    {
        ArgumentNullException.ThrowIfNull(gs);
        Current.Dispose();
        Current = gs;
    }

    public GameState() : this(OfficialServersUrl)
    { }

    public GameState(string serverAddress)
    {
        Connection = new(serverAddress);
    }

    public GameState(string serverAddress, TimeSpan HttpTimeout)
    {
        Connection = new(serverAddress, HttpTimeout);
    }

    public void ChangeAccessToken(string accessToken)
    {
        TokenStore.AccessToken = accessToken;
        Connection.AccessToken = accessToken;
    }

    public void Dispose()
    {
        if (this == Current)
            Current = null!;

        Connection.Dispose();
    }
}

