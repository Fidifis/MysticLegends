namespace MysticLegendsClient;

public class ItemDropContext
{
    public IItemDrop Owner { get; set; }
    public int ContextId { get; set; }

    public ItemDropContext(IItemDrop owner, int id)
    {
        Owner = owner;
        ContextId = id;
    }
}
