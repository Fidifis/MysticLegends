using MysticLegendsShared.Models;

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

public class ItemViewRelation
{
    public ItemSlot ManagedSlot { get; private set; }
    public ItemSlot TransitSlot { get; private set; }

    public ItemViewRelation(ItemSlot managedSlot, ItemSlot transitSlot)
    {
        ManagedSlot = managedSlot;
        TransitSlot = transitSlot;
    }
}

public class ItemDropEventArgs: EventArgs
{

    public ItemSlot FromSlot { get; private set; }
    public ItemSlot ToSlot { get; private set; }

    public ItemDropEventArgs(ItemSlot from, ItemSlot to)
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

    public ICollection<ItemViewRelation> ViewRelations { get; init; }
    public static void EstablishRelation(ItemSlot managedSlot, ItemSlot transitSlot)
    {
        var relation = new ItemViewRelation(managedSlot, transitSlot);
        managedSlot.Owner.AddRelation(relation);
        transitSlot.Owner.AddRelation(relation);
    }
    public virtual void AddRelation(ItemViewRelation relation) => ViewRelations.Add(relation);

    public virtual void ReleaseFromManaged(ItemSlot managed) => ViewRelations.Remove(GetRelationBySlot(managed)!);
    public virtual void FulfillFromManaged(ItemSlot managed) => ViewRelations.Remove(GetRelationBySlot(managed)!);
    public virtual void ReleaseFromTransit(ItemSlot transit) => ViewRelations.Remove(GetRelationBySlot(transit)!);
    public virtual void FulfillFromTransit(ItemSlot transit) => ViewRelations.Remove(GetRelationBySlot(transit)!);

    public static void AbortTransition(IItemView view)
    {
        var relations = new List<ItemViewRelation>(view.ViewRelations);
        foreach (var relation in relations)
        {
            relation.ManagedSlot.Owner.ReleaseFromManaged(relation.ManagedSlot);
            relation.TransitSlot.Owner.ReleaseFromTransit(relation.TransitSlot);
        }
    }
    public static void FulfillTransition(IItemView view)
    {
        var relations = new List<ItemViewRelation>(view.ViewRelations);
        foreach (var relation in relations)
        {
            relation.ManagedSlot.Owner.FulfillFromManaged(relation.ManagedSlot);
            relation.TransitSlot.Owner.FulfillFromTransit(relation.TransitSlot);
        }
    }

    public virtual ItemViewRelation? GetRelationBySlot(ItemSlot slot) => ViewRelations.FirstOrDefault((relation) => relation.ManagedSlot == slot || relation.TransitSlot == slot);
}

public interface IItemViewLogicHandler
{
    
}
