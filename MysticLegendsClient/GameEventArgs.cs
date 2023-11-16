namespace MysticLegendsClient;

public class UpdateEventArgs<T> : EventArgs
{
    public T Value { get; init; }

    public UpdateEventArgs(T value)
    {
        Value = value;
    }
}
