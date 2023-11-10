namespace MysticLegendsClient;

internal class GameState : IDisposable
{
    public static GameState Current { get; private set; } = new();
    public ApiClient Connection { get; private init; }
    public TokenStore TokenStore { get; private init; } = new();

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
        Connection = new(serverAddress, TokenStore);
    }

    public GameState(string serverAddress, TimeSpan HttpTimeout)
    {
        Connection = new(serverAddress, TokenStore, HttpTimeout);
    }

    public void Dispose()
    {
        if (this == Current)
            Current = null!;

        Connection.Dispose();
    }
}

