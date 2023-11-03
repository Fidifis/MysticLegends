namespace MysticLegendsClient;

internal class CityWindowClosedEventArgs: EventArgs
{
    public CityWindow ClosedWindow { get; init; }

    public CityWindowClosedEventArgs(CityWindow closedWindow)
    {
        ClosedWindow = closedWindow;
    }
}

internal class CurrencyUpdateEventArgs: EventArgs
{
    public int Value { get; init; }

    public CurrencyUpdateEventArgs(int value)
    {
        Value = value;
    }
}
