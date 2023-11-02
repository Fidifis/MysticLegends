namespace MysticLegendsClient;

public interface IDataView<TData>
{
    public TData? Data { get; set; }
    public void Update();
}

public interface IDataViewWithDrop<TData, TElement> : IDataView<TData>, IItemDrop
{
    public TElement? GetByContextId(int id);
}
