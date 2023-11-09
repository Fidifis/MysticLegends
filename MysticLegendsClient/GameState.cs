namespace MysticLegendsClient;

internal class GameState : IDisposable
{
    public static GameState Current { get; private set; } = new();
    public ApiClient Connection { get; private init; }
    public TokenStore TokenStore { get; private init; } = new();

    public GameEvents GameEvents { get; private init; } = new();

    public static void MakeGameStateCurrent(GameState gs)
    {
        ArgumentNullException.ThrowIfNull(gs);
        Current.Dispose();
        Current = gs;
    }

    public GameState() : this("http://localhost:5281")
    { }

    public GameState(string serverAddress)
    {
        Connection = new(serverAddress);
    }

    public void Dispose()
    {
        if (this == Current)
            Current = null!;

        Connection.Dispose();
    }
}

