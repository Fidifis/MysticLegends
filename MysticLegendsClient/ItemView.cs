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
    public ItemSlot ManagedSlot { get; set; }
    public ItemSlot TransitSlot { get; set; }

    public ItemViewRelation(ItemSlot managedSlot, ItemSlot transitSlot)
    {
        ManagedSlot = managedSlot;
        TransitSlot = transitSlot;
    }

    public static bool EstablishRelation(ItemSlot managedSlot, ItemSlot transitSlot)
    {
        if (!managedSlot.Owner.CanTransitItems || !transitSlot.Owner.CanTransitItems)
            return false;
            //throw new Exception("Both sides must allow item transition");

        var relation = new ItemViewRelation(managedSlot, transitSlot);
        managedSlot.Owner.AddRelation(relation);
        transitSlot.Owner.AddRelation(relation);
        return true;
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
    //public IReadOnlyCollection<InventoryItem> Items { get; set; }
    public IEnumerable<InventoryItem> Items { get; set; }
    //public void Update();
    public void AddItem(InventoryItem item);
    public void UpdateItem(InventoryItem updatedItem);

    public event ItemDropEventHandler? ItemDropEvent;
    public void InvokeItemDropEvent(IItemView sender, ItemDropEventArgs args);

    //public ItemSlot GetSlotByPosition(int position);

    public static void DropEventHandover(ItemDropEventArgs args)
    {
        if (args.IsHandover) return;
        args.IsHandover = true;
        args.FromSlot.Owner.InvokeItemDropEvent(args.ToSlot.Owner, args);
    }

    public bool CanTransitItems { get; }
    //public ICollection<ItemViewRelation> ViewRelations { get; }
    public void AddRelation(ItemViewRelation relation);
    public void RemoveRelationFromManaged(ItemSlot managed);
    public void RemoveRelationFromTransit(ItemSlot transit);

    public void CloseRelation(ItemViewRelation relation);
    public void CloseRelations();

    public ItemViewRelation? GetRelationBySlot(ItemSlot slot);

    public static bool IsSlotLocked(ItemSlot slot) => slot.Owner.GetRelationBySlot(slot) is not null;
}

public abstract class ItemViewUserControl : UserControl, IItemView
{
    public abstract IEnumerable<InventoryItem> Items { get; set; }

    //public abstract void Update();
    public abstract void AddItem(InventoryItem item);
    public abstract void UpdateItem(InventoryItem updatedItem);

    public event IItemView.ItemDropEventHandler? ItemDropEvent;
    public void InvokeItemDropEvent(IItemView sender, ItemDropEventArgs args) => ItemDropEvent?.Invoke(sender, args);

    //public abstract ItemSlot GetSlotByPosition(int position);

    public virtual bool CanTransitItems { get; set; } = false;
    protected LinkedList<ItemViewRelation> ViewRelations { get; init; } = new();

    public virtual void AddRelation(ItemViewRelation relation) { }
    public virtual void RemoveRelationFromManaged(ItemSlot managed) { }
    public virtual void RemoveRelationFromTransit(ItemSlot transit) { }

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

    public void CloseRelation(ItemViewRelation relation)
    {
        relation.ManagedSlot.Owner.RemoveRelationFromManaged(relation.ManagedSlot);
        relation.TransitSlot.Owner.RemoveRelationFromTransit(relation.TransitSlot);
    }

    public void CloseRelations()
    {
        var relations = new List<ItemViewRelation>(ViewRelations);
        foreach (var relation in relations)
        {
            CloseRelation(relation);
        }
    }
}
