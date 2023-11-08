using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using MysticLegendsClient.Resources;
using System.Windows.Input;
using MysticLegendsShared.Models;
using MysticLegendsShared.Utilities;

namespace MysticLegendsClient.Controls
{
    /// <summary>
    /// Interakční logika pro InventoryView.xaml
    /// </summary>
    public partial class InventoryView : ItemViewUserControl
    {
        private readonly struct SlotTuple
        {
            public ItemSlot ItemSlot { get; init; }
            public Image Image { get; init; }
            public Label Label { get; init; }
            public FrameworkElement Root { get; init; }
        }

        public InventoryView(): this(false)
        { }

        public InventoryView(bool canTransitItems)
        {
            InitializeComponent();
            DataContext = this;
            CanTransitItems = canTransitItems;
        }

        public int Capacity { get; private set; } = -1;
        private bool InfinityMode => Capacity == -1;

        private int ItemCount => ItemSlots.Where(slot => slot.ItemSlot.Item is not null).Count();
        private string CapacityCounter => InfinityMode ? ItemCount.ToString() : $"{ItemCount}/{Capacity}";

        public override IReadOnlyCollection<InventoryItem> Items
        {
            get => ItemSlots.Where((slot) => slot.ItemSlot.Item is not null).Select((slot) => slot.ItemSlot.Item!).ToList();
            set => FillData(value);
        }

        private readonly List<SlotTuple> ItemSlots = new();

        public override bool CanTransitItems { get; set; } = false;

        private static readonly DependencyProperty counterVisibility = DependencyProperty.Register("ShowCounter", typeof(Visibility), typeof(InventoryView));
        public Visibility CounterVisibility
        {
            get { return (Visibility)GetValue(counterVisibility); }
            set { SetValue(counterVisibility, value); }
        }

        private ItemSlot? GetSlotByItem(InventoryItem item) => ItemSlots.Where(slot => slot.ItemSlot.Item == item).Select(slot => slot.ItemSlot).FirstOrDefault();
        //private SlotTuple GetRecordBySlot(ItemSlot itemSlot) => ItemSlots.FirstOrDefault(slot => slot.ItemSlot == itemSlot);

        public override void AddItem(InventoryItem item)
        {
            if (InfinityMode)
            {
                UpdateSlots(ItemCount + 1);
                SetSlot(ItemCount, item);
            }
            else
                if (item.Position < ItemSlots.Count)
                    SetSlot(item.Position, item);
        }

        public override void UpdateItem(InventoryItem updatedItem) =>
            SwapItems(GetSlotByItem(updatedItem)!, ItemSlots[updatedItem.Position].ItemSlot);

        public void FillData(IReadOnlyCollection<InventoryItem> items, int capacity)
        {
            Capacity = capacity;
            FillData(items);
        }

        private void FillData(IReadOnlyCollection<InventoryItem> items)
        {
            var capacity = InfinityMode ? items.Count : Capacity;

            UpdateSlots(capacity);
            for (int i = 0; i < ItemSlots.Count; i++)
            {
                SetSlot(i, null);
            }

            int fi = 0;
            foreach (var item in items)
            {
                if (InfinityMode)
                    SetSlot(fi++, item);
                else
                    if (item.Position < ItemSlots.Count)
                    SetSlot(item.Position, item);
            }

            UpdateCapacityCounter();
        }

        private void SwapItemsData(ItemSlot slot1, ItemSlot slot2)
        {
            var item = slot1.Item;
            slot1.Item = slot2.Item;
            slot2.Item = item;
        }

        private void SwapItemsPostion(ItemSlot slot1, ItemSlot slot2)
        {
            var position = slot1.GridPosition;
            if (slot1.Item is not null)
                slot1.Item.Position = slot2.GridPosition;
            if (slot2.Item is not null)
                slot2.Item.Position = position;
        }

        private void SwapItemsImage(ItemSlot slot1, ItemSlot slot2)
        {
            var image = ItemSlots[slot1.GridPosition].Image.Source;

            ItemSlots[slot1.GridPosition].Image.Source = ItemSlots[slot2.GridPosition].Image.Source;
            ItemSlots[slot2.GridPosition].Image.Source = image;
        }

        private void SwapItemsRelation(ItemViewRelation? relation, ItemSlot slot1, ItemSlot slot2)
        {
            
            if (relation is not null)
            {
                if (relation.TransitSlot == slot1)
                {
                    relation.TransitSlot = slot2;
                }
                else if (relation.ManagedSlot == slot1)
                {
                    relation.ManagedSlot = slot2;
                }
            }
        }

        public void SwapItems(ItemSlot slot1, ItemSlot slot2)
        {
            SwapItemsData(slot1, slot2);
            SwapItemsImage(slot1, slot2);
            SwapItemsPostion(slot1, slot2);

            var relation1 = GetRelationBySlot(slot1);
            var relation2 = GetRelationBySlot(slot2);
            SwapItemsRelation(relation1, slot1, slot2);
            SwapItemsRelation(relation2, slot2, slot1);
        }

        private FrameworkElement CreateSlot(out Image image, out Label label)
        {
            label = new Label
            {
                FontSize = 20,
                HorizontalContentAlignment = HorizontalAlignment.Right,
                VerticalContentAlignment = VerticalAlignment.Bottom,
                Margin = new Thickness(3),
            };
            image = new Image
            {
                Width = 75,
                Height = 75,
            };
            var border = new Border
            {
                Margin = new Thickness(5),
                BorderThickness = new Thickness(3),
                BorderBrush = Brushes.Black,
                Child = image,
            };

            var grid = new Grid()
            {
                AllowDrop = true,
            };
            grid.Drop += Image_Drop;
            grid.MouseLeftButtonDown += Image_MouseLeftButtonDown;

            grid.Children.Add(label);
            grid.Children.Add(border);

            ToolTipService.SetInitialShowDelay(grid, 0);

            return grid;
        }

        private void RemoveSlotRange(int index, int count)
        {
            inventoryPanel.Children.RemoveRange(index, count);
            ItemSlots.RemoveRange(index, count);
        }

        private void AddSlotToView(FrameworkElement element, Image image, Label label)
        {
            inventoryPanel.Children.Add(element);
            var itemSlot = new ItemSlot(this, ItemSlots.Count);
            ItemSlots.Add(new() { ItemSlot = itemSlot, Image = image, Label = label, Root = element });
            element.Tag = itemSlot;
        }

        private void UpdateSlots(int count)
        {
            Debug.Assert(ItemSlots.Count == inventoryPanel.Children.Count, "list counts differ");
            if (count > ItemSlots.Count)
                for (int i = ItemSlots.Count; i < count; i++)
                    AddSlotToView(CreateSlot(out Image image, out Label label), image, label);
            else if (count < ItemSlots.Count)
                RemoveSlotRange(count, ItemSlots.Count - count);
        }

        private void UpdateCapacityCounter()
        {
            capacityCounter.Content = CapacityCounter;
        }

        private void SetSlot(int index, InventoryItem? item)
        {
            ItemSlots[index].ItemSlot.Item = item;
            ItemSlots[index].Image.Source = item is null ? null : BitmapTools.FromResource(ItemIcons.ResourceManager.GetString(item.Item.Icon)!);
            ItemSlots[index].Label.Content = item?.StackCount == 1 ? "" : item?.StackCount.ToString();
            ItemSlots[index].Root.ToolTip = ItemToolTip.Create(item);
        }

        //public override ItemSlot GetSlotByPosition(int position) => ItemSlots.Where((slot) => slot.Item1.GridPosition == position).Select((record) => record.Item1).First();

        private void ItemLockVisual(int position, bool isLocked)
        {
            ItemSlots[position].Image.Opacity = isLocked ? 0.2 : 1;
        }

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (((FrameworkElement)sender).Tag is ItemSlot slot && ItemSlots[slot.GridPosition].ItemSlot.Item is not null)
            {
                HandleDrag(slot);
            }
        }

        private void Image_Drop(object sender, DragEventArgs e)
        {
            var target = (FrameworkElement)sender;
            var targetSlot = (ItemSlot)target.Tag;
            HandleDrop(targetSlot, e);
        }

        public override void AddRelation(ItemViewRelation relation)
        {
            ViewRelations.AddLast(relation);

            if (relation.ManagedSlot.Owner == this)
            {
                ItemLockVisual(relation.ManagedSlot.Item!.Position, true);
            }
            else if (relation.TransitSlot.Owner == this)
            {
                SetSlot(relation.TransitSlot.GridPosition, relation.TransitSlot.Item!);
            }
            else Debug.Assert(false);
        }

        public override void RemoveRelationFromManaged(ItemSlot managed)
        {
            ViewRelations.Remove(GetRelationBySlot(managed)!);
            ItemLockVisual(managed.GridPosition, false);
        }

        public override void RemoveRelationFromTransit(ItemSlot transit)
        {
            ViewRelations.Remove(GetRelationBySlot(transit)!);
            SetSlot(transit.GridPosition, null);
        }
    }
}
