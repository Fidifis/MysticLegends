namespace MysticLegendsClient;

public interface IItemDrop
{
    public delegate void ItemDropEventHandler(ItemDropContext source, ItemDropContext target);
    public ItemDropEventHandler? ItemDropTargetCallback { get; set; }
    public ItemDropEventHandler? ItemDropSourceCallback { get; set; }
}
