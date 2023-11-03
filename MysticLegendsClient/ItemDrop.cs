namespace MysticLegendsClient;

public interface IItemDrop
{
    public delegate void ItemDrop(ItemDropEventArgs source, ItemDropEventArgs target);
    public ItemDrop? ItemDropCallback { get; set; }
}
