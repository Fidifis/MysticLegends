using MysticLegendsShared.Models;
using System.Windows;
using System.Windows.Controls;

namespace MysticLegendsClient;

public class ItemSlot
{
    public InventoryItem? Item { get; set; }
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
    public ItemSlot FromSlot { get; init; }
    public ItemSlot ToSlot { get; init; }
    public bool IsHandover { get; set; } = false;

    public ItemDropEventArgs(ItemSlot from, ItemSlot to)
    {
        FromSlot = from;
        ToSlot = to;
    }
}

public interface IItemView
{
    public delegate void ItemDropEventHandler(IItemView sender, ItemDropEventArgs args);
    public ICollection<InventoryItem> Items { get; set; }
    public void Update();

    public event ItemDropEventHandler? ItemDropEvent;
    public void InvokeItemDropEvent(IItemView sender, ItemDropEventArgs args);

    //public ItemSlot GetSlotByPosition(int position);

    public static void DropEventHandover(ItemDropEventArgs args)
    {
        if (args.IsHandover) return;
        args.IsHandover = true;
        args.FromSlot.Owner.InvokeItemDropEvent(args.ToSlot.Owner, args);
    }

    public static void EstablishRelation(ItemSlot managedSlot, ItemSlot transitSlot)
    {
        if (!managedSlot.Owner.CanTransitItems || !transitSlot.Owner.CanTransitItems)
            throw new Exception("Both sides must allow item transition");

        var relation = new ItemViewRelation(managedSlot, transitSlot);
        managedSlot.Owner.AddRelation(relation);
        transitSlot.Owner.AddRelation(relation);
    }
    public bool CanTransitItems { get; }
    public ICollection<ItemViewRelation> ViewRelations { get; }
    public void AddRelation(ItemViewRelation relation);

    public void RemoveFromManaged(ItemSlot managed);
    public void RemoveFromTransit(ItemSlot transit);

    public static void CloseTransition(IItemView view)
    {
        var relations = new List<ItemViewRelation>(view.ViewRelations);
        foreach (var relation in relations)
        {
            relation.ManagedSlot.Owner.RemoveFromManaged(relation.ManagedSlot);
            relation.TransitSlot.Owner.RemoveFromTransit(relation.TransitSlot);
        }
    }

    public ItemViewRelation? GetRelationBySlot(ItemSlot slot);
}

public abstract class ItemViewUserControl : UserControl, IItemView
{
    public abstract ICollection<InventoryItem> Items { get; set; }
    public abstract void Update();

    public event IItemView.ItemDropEventHandler? ItemDropEvent;
    public void InvokeItemDropEvent(IItemView sender, ItemDropEventArgs args) => ItemDropEvent?.Invoke(sender, args);

    //public abstract ItemSlot GetSlotByPosition(int position);

    public virtual bool CanTransitItems { get; set; } = false;
    public ICollection<ItemViewRelation> ViewRelations { get; init; } = new LinkedList<ItemViewRelation>();
    public virtual void AddRelation(ItemViewRelation relation) { }
    public virtual void RemoveFromManaged(ItemSlot managed) { }
    public virtual void RemoveFromTransit(ItemSlot transit) { }

    public virtual ItemViewRelation? GetRelationBySlot(ItemSlot slot) => ViewRelations.FirstOrDefault((relation) => relation.ManagedSlot == slot || relation.TransitSlot == slot);

    protected virtual void HandleDrag(ItemSlot itemSlot)
    {
        var data = new DataObject(typeof(ItemSlot), itemSlot);
        DragDrop.DoDragDrop(this, data, DragDropEffects.Move);
    }

    protected virtual void HandleDrop(ItemSlot itemSlot, DragEventArgs e)
    {
        if (e.Data.GetDataPresent(typeof(ItemSlot)))
        {
            var sourceSlot = (ItemSlot)e.Data.GetData(typeof(ItemSlot));
            var targetSlot = itemSlot;

            InvokeItemDropEvent(sourceSlot.Owner, new ItemDropEventArgs(sourceSlot, targetSlot));
        }
    }
}
