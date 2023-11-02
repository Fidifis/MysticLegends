namespace MysticLegendsClient;

public interface IItemDrop
{
    public delegate void ItemDrop(ItemDropContext source, ItemDropContext target);
    public ItemDrop? ItemDropCallback { get; set; }
}
