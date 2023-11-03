namespace MysticLegendsClient;

public class ItemDropEventArgs: EventArgs
{
    public IItemDrop Owner { get; set; }
    public int ContextId { get; set; }

    public ItemDropEventArgs(IItemDrop owner, int id)
    {
        Owner = owner;
        ContextId = id;
    }
}
