namespace MysticLegendsClient;

public interface IViewableItem
{
    public int Id { get; }
    public string Icon { get; }
    public int StackNumber { get; }
    public int Position { get; }
}

public class ItemSlot
{
    public IViewableItem? Item { get; set; }
    public IItemView Owner { get; set; }
    public int GridPosition { get; set; }

    public ItemSlot(IItemView owner, int gridPosition)
    {
        Owner = owner;
        GridPosition = gridPosition;
    }
}

public class ItemDropEventArgs: EventArgs
{

    public ItemSlot FromSlot { get; set; }
    public ItemSlot ToSlot { get; set; }

    public ItemDropEventArgs( ItemSlot from, ItemSlot to)
    {
        FromSlot = from;
        ToSlot = to;
    }
}

public interface IItemView
{
    public delegate void ItemDropEventHandler(IItemView sender, ItemDropEventArgs args);
    public ICollection<IViewableItem> Items { get; set; }
    public void Update();
    public event ItemDropEventHandler? ItemDropEvent;
    //public IItemViewLogicHandler LogicHandler { get; set; }

    //public IDataItem GetById(int id);
    //public IDataItem GetByGridPosition(int positon);
}

public interface IItemViewLogicHandler
{
    
}

//public interface IDataViewWithDrop<TData, TElement> : IDataView<TData>, IItemDrop
//{
//    public TElement? GetByContextId(int id);
//}
