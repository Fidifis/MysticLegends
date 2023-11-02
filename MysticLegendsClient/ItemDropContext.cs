namespace MysticLegendsClient;

public struct ItemDropContext
{
    public IItemDrop Owner { get; set; }
    public int Id { get; set; }

    public ItemDropContext(IItemDrop owner, int id)
    {
        Owner = owner;
        Id = id;
    }
}
