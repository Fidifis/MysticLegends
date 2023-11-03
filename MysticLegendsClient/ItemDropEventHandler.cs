namespace MysticLegendsClient;

public interface IItemDrop
{
    public delegate void ItemDropEventHandler(ItemDropContext source, ItemDropContext target);
    public ItemDropEventHandler? ItemDropCallback { get; set; }
}
