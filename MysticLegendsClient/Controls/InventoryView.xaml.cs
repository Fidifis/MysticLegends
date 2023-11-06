using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using MysticLegendsClient.Resources;
using System.Windows.Input;

namespace MysticLegendsClient.Controls
{
    /// <summary>
    /// Interakční logika pro InventoryView.xaml
    /// </summary>
    public partial class InventoryView : UserControl, IItemView
    {
        public InventoryView(): this(false)
        { }

        public InventoryView(bool canTransitItems)
        {
            InitializeComponent();
            DataContext = this;
            CanTransitItems = canTransitItems;
        }

        public event IItemView.ItemDropEventHandler? ItemDropEvent;

        public int Capacity { get; private set; } = -1;

        private readonly List<Tuple<ItemSlot, Image, Label>> ItemSlots = new();

        public bool CanTransitItems { get; init; } = false;
        public ICollection<ItemViewRelation> ViewRelations { get; init; } = new List<ItemViewRelation>();

        private static readonly DependencyProperty counterVisibility = DependencyProperty.Register("ShowCounter", typeof(Visibility), typeof(InventoryView));

        public Visibility CounterVisibility
        {
            get { return (Visibility)GetValue(counterVisibility); }
            set { SetValue(counterVisibility, value); }
        }

        private int ItemCount => ItemSlots.Count;
        private string CapacityCounter => $"{ItemCount}/{Capacity}";

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

            return grid;
        }

        private void RemoveSlotRange(int index, int count)
        {
            inventoryPanel.Children.RemoveRange(index, count);
            ItemSlots.RemoveRange(index, count);
        }

        private void AddToSlot(FrameworkElement element, Image image, Label label)
        {
            inventoryPanel.Children.Add(element);
            var itemSlot = new ItemSlot(this, ItemSlots.Count - 1);
            ItemSlots.Add(new (itemSlot, image, label));
            element.Tag = itemSlot;
        }

        private void UpdateSlots(int count)
        {
            Debug.Assert(ItemSlots.Count == inventoryPanel.Children.Count, "list counts differ");
            if (count > ItemSlots.Count)
                for (int i = ItemSlots.Count; i < count; i++)
                    AddToSlot(CreateSlot(out Image image, out Label label), image, label);
            else if (count < ItemSlots.Count)
                RemoveSlotRange(count, ItemSlots.Count - count);
        }

        private void UpdateCapacityCounter()
        {
            capacityCounter.Content = CapacityCounter;
        }

        public void FillData(ICollection<IViewableItem> items, int capacity)
        {
            Capacity = capacity;
            PutItems(items);
        }

        public void PutItems(ICollection<IViewableItem> items)
        {
            var infiniteMode = Capacity == -1;
            var capacity = infiniteMode ? items.Count() : Capacity;

            UpdateSlots(capacity);
            for (int i = 0; i < ItemSlots.Count; i++)
            {
                SetSlot(i, null);
                
            }

            int fi = 0;
            foreach (var item in items)
            {
                if (infiniteMode)
                    SetSlot(fi++, item);
                else
                    if (item.Position < ItemSlots.Count)
                        SetSlot(item.Position, item);
            }

            UpdateCapacityCounter();
        }

        private void SetSlot(int index, IViewableItem? item)
        {
            ItemSlots[index].Item1.Item = item;
            ItemSlots[index].Item2.Source = item is null ? null : BitmapTools.FromResource(ItemIcons.ResourceManager.GetString(item.Icon)!);
            ItemSlots[index].Item3.Content = item?.StackNumber == 1 ? "" : item?.StackNumber.ToString();
        }


        private void ItemLockVisual(int position, bool isLocked)
        {
            ItemSlots[position].Item2.Opacity = isLocked ? 0.2 : 1;
        }

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (((FrameworkElement)sender).Tag is ItemSlot slot && ItemSlots[slot.GridPosition].Item1.Item is not null)
            {
                var data = new DataObject(typeof(ItemSlot), slot);
                DragDrop.DoDragDrop((DependencyObject)sender, data, DragDropEffects.Move);
            }
        }

        private void Image_Drop(object sender, DragEventArgs e)
        {
            var target = (FrameworkElement)sender;

            if (e.Data.GetDataPresent(typeof(ItemSlot)))
            {
                var sourceSlot = (ItemSlot)e.Data.GetData(typeof(ItemSlot));
                var targetSlot = (ItemSlot)target.Tag;

                ItemDropEvent?.Invoke(sourceSlot.Owner, new ItemDropEventArgs(sourceSlot, targetSlot));
            }
        }

        public void AddRelation(ItemViewRelation relation)
        {
            ViewRelations.Add(relation);

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

        public void RemoveFromManaged(ItemSlot managed)
        {
            ViewRelations.Remove(((IItemView)this).GetRelationBySlot(managed)!);
            ItemLockVisual(managed.GridPosition, false);
        }

        public void RemoveFromTransit(ItemSlot transit)
        {
            ViewRelations.Remove(((IItemView)this).GetRelationBySlot(transit)!);
            SetSlot(transit.GridPosition, transit.Item!);
        }
    }
}
