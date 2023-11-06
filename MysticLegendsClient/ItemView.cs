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
    public ItemSlot ManagedSlot { get; init; }
    public ItemSlot TransitSlot { get; init; }

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
    //public IEnumerable<IViewableItem> Items => ItemSlots.Where(slot => slot.Item is not null).Select(slot => slot.Item!);  // TODO Maybe useless
    //public IEnumerable<ItemSlot> ItemSlots { get; } // TODO Maybe useless
    public void PutItems(ICollection<IViewableItem> items); // TODO Maybe useless
    public event ItemDropEventHandler? ItemDropEvent;

    //public IViewableItem GetItemByGridPosition();
    

    public bool CanTransitItems { get => false; }
    public ICollection<ItemViewRelation> ViewRelations { get; }
    public static void EstablishRelation(ItemSlot managedSlot, ItemSlot transitSlot)
    {
        if (!managedSlot.Owner.CanTransitItems || !transitSlot.Owner.CanTransitItems)
            throw new Exception("Both sides must allow item transition");

        var relation = new ItemViewRelation(managedSlot, transitSlot);
        managedSlot.Owner.AddRelation(relation);
        transitSlot.Owner.AddRelation(relation);
    }
    public void AddRelation(ItemViewRelation relation) { }

    public void RemoveFromManaged(ItemSlot managed) { }
    public void RemoveFromTransit(ItemSlot transit) { }

    public static void CloseTransition(IItemView view)
    {
        var relations = new List<ItemViewRelation>(view.ViewRelations);
        foreach (var relation in relations)
        {
            relation.ManagedSlot.Owner.RemoveFromManaged(relation.ManagedSlot);
            relation.TransitSlot.Owner.RemoveFromTransit(relation.TransitSlot);
        }
    }

    public ItemViewRelation? GetRelationBySlot(ItemSlot slot) => ViewRelations.FirstOrDefault((relation) => relation.ManagedSlot == slot || relation.TransitSlot == slot);
}
