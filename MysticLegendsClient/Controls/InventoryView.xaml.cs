using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using MysticLegendsClient.Resources;
using System.Windows.Input;
using MysticLegendsShared.Utilities;
using MysticLegendsShared.Models;
using System.Windows.Media.Imaging;
using System.Collections;

namespace MysticLegendsClient.Controls
{
    /// <summary>
    /// Interakční logika pro InventoryView.xaml
    /// </summary>
    public partial class InventoryView : UserControl, IItemView
    {
        public InventoryView()
        {
            InitializeComponent();
            DataContext = this;
        }

        public InventoryView(FrameworkElement owner)
        {
            InitializeComponent();
            DataContext = this;
        }

        public event IItemView.ItemDropEventHandler? ItemDropEvent;

        public ICollection<IViewableItem> Items { get; set; } = new List<IViewableItem>();

        public ICollection<ItemViewRelation> ViewRelations { get; init; } = new List<ItemViewRelation>();

        public void Update() => FillData(Items);


        private static readonly DependencyProperty counterVisibility = DependencyProperty.Register("ShowCounter", typeof(Visibility), typeof(InventoryView));

        public Visibility CounterVisibility
        {
            get { return (Visibility)GetValue(counterVisibility); }
            set { SetValue(counterVisibility, value); }
        }

        private int ItemCount { get => ItemSlots.Count(item => item.Item1.Source is not null); }
        private int Capacity { get => ItemSlots.Count; }
        private string CapacityCounter { get => $"{ItemCount}/{Capacity}"; }

        private readonly List<Tuple<Image, Label>> ItemSlots = new();
        [Obsolete]
        private readonly List<Tuple<object, int>> LockedItems = new();

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
            ItemSlots.Add(new (image, label));
            element.Tag = new ItemSlot(this, ItemSlots.Count - 1);
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

        private void FillData(ICollection<IViewableItem> inventoryData)
        {
            var inventoryItems = inventoryData;
            var inventoryCapacity = inventoryData.Count;

            var infiniteMode = inventoryCapacity == -1;
            var capacity = infiniteMode ? inventoryItems.Count : inventoryCapacity;

            UpdateSlots(capacity);
            for (int i = 0; i < ItemSlots.Count; i++)
            {
                ItemSlots[i].Item1.Source = null;
                ItemSlots[i].Item2.Content = "";
            }

            int fi = 0;
            foreach (var item in inventoryItems)
            {
                var stackCountStr = item.StackNumber == 1 ? "" : item.StackNumber.ToString();
                if (infiniteMode)
                    SetSlot(fi++, item.Icon, stackCountStr);
                else
                    if (item.Position < ItemSlots.Count)
                        SetSlot(item.Position, item.Icon, stackCountStr);
            }

            UpdateCapacityCounter();
        }

        private void SetSlot(int index, string icon, string labelString)
        {
            ItemSlots[index].Item1.Source = BitmapTools.FromResource(ItemIcons.ResourceManager.GetString(icon)!);
            ItemSlots[index].Item2.Content = labelString;
        }

        public void AddRelation(ItemViewRelation relation)
        {
            ViewRelations.Add(relation);

            if (relation.ManagingView == this)
            {
                var item = relation.ManagingView == this ? relation.ManagedSlot : relation.TransitSlot;
                ItemLockVisual(item.Position, true);
            }
            else if (relation.TransitingView == this)
            {
                Items.Add(relation.TransitSlot);
                Update();
            }
            else Debug.Assert(false);
        }

        public void ReleaseItem(IViewableItem item)
        {
            ItemLockVisual(item.Position, false);
        }

        public virtual void TransitItem(IViewableItem item)
        {
            ItemLockVisual(item.Position, false);

        }


        private void ItemLockVisual(int position, bool isLocked)
        {
            ItemSlots[position].Item1.Opacity = isLocked ? 0.2 : 1;
        }

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (((FrameworkElement)sender).Tag is ItemSlot slot && ItemSlots[slot.GridPosition].Item1.Source is not null)
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
    }
}
