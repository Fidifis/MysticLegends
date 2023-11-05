namespace MysticLegendsClient;

internal class CurrencyUpdateEventArgs: EventArgs
{
    public int Value { get; init; }

    public CurrencyUpdateEventArgs(int value)
    {
        Value = value;
    }
}
