using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using MysticLegendsClient.Resources;
using System.Windows.Input;
using MysticLegendsShared.Utilities;
using MysticLegendsShared.Models;
using System.Windows.Media.Imaging;

namespace MysticLegendsClient.Controls
{
    /// <summary>
    /// Interakční logika pro InventoryView.xaml
    /// </summary>
    public partial class InventoryView : UserControl, IDataViewWithDrop<IInventory, InventoryItem>
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
            Owner = owner;
        }

        public FrameworkElement? Owner { get; set; }

        private static readonly DependencyProperty counterVisibility = DependencyProperty.Register("ShowCounter", typeof(Visibility), typeof(InventoryView));

        public Visibility CounterVisibility
        {
            get { return (Visibility)GetValue(counterVisibility); }
            set { SetValue(counterVisibility, value); }
        }

        public IItemDrop.ItemDropEventHandler? ItemDropTargetCallback { get; set; }
        public IItemDrop.ItemDropEventHandler? ItemDropSourceCallback { get; set; }

        private int ItemCount { get => ImgSlots.Count(item => item.Source is not null); }
        private int Capacity { get => ImgSlots.Count; }
        private string CapacityCounter { get => $"{ItemCount}/{Capacity}"; }

        private readonly List<Image> ImgSlots = new();
        private readonly List<Tuple<object, int>> LockedItems = new();

        private IInventory? data;
        public IInventory? Data
        {
            get => data;
            set
            {
                data = value;
                FillData(data);
            }
        }

        public void Update() => FillData(data);

        public InventoryItem? GetByContextId(int id) => Data?.InventoryItems?.Where(item => item.Position == id).FirstOrDefault();


        private FrameworkElement CreateSlot(out Image image)
        {
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

            grid.Children.Add(new Label());
            grid.Children.Add(border);

            return grid;
        }

        private void RemoveSlotRange(int index, int count)
        {
            inventoryPanel.Children.RemoveRange(index, count);
            ImgSlots.RemoveRange(index, count);
        }

        private void AddToSlot(FrameworkElement element, Image image)
        {
            inventoryPanel.Children.Add(element);
            ImgSlots.Add(image);
            element.Tag = new ItemDropContext(this, ImgSlots.Count - 1);
        }

        private void UpdateSlots(int count)
        {
            Debug.Assert(ImgSlots.Count == inventoryPanel.Children.Count, "list counts differ");
            if (count > ImgSlots.Count)
                for (int i = ImgSlots.Count; i < count; i++)
                    AddToSlot(CreateSlot(out Image image), image);
            else if (count < ImgSlots.Count)
                RemoveSlotRange(count, ImgSlots.Count - count);
        }

        private void UpdateCapacityCounter()
        {
            capacityCounter.Content = CapacityCounter;
        }

        private void FillData(IInventory? inventoryData)
        {
            var inventoryItems = inventoryData?.InventoryItems ?? new List<InventoryItem>();
            var inventoryCapacity = inventoryData?.Capacity ?? 0;

            var infiniteMode = inventoryCapacity == -1;
            var capacity = infiniteMode ? inventoryItems.Count : inventoryCapacity;

            UpdateSlots(capacity);
            for (int i = 0; i < ImgSlots.Count; i++)
                ImgSlots[i].Source = null;

            int fi = 0;
            foreach (var item in inventoryItems)
            {
                if (infiniteMode)
                    ImgSlots[fi++].Source = BitmapTools.FromResource(ItemIcons.ResourceManager.GetString(item.Item.Icon)!);
                else
                    if (item.Position < ImgSlots.Count)
                        ImgSlots[item.Position].Source = BitmapTools.FromResource(ItemIcons.ResourceManager.GetString(item.Item.Icon)!);
            }

            UpdateCapacityCounter();
        }

        public void LockItem(object owner, int itemId)
        {
            var item = (Data?.InventoryItems.Where(item => item.InvitemId == itemId).FirstOrDefault()) ?? throw new Exception("Item not found");
            var img = ImgSlots[item.Position];
            LockedItems.Add(new (owner, item.Position));
            img.Opacity = 0.2;
        }

        public void ReleaseLock(object owner)
        {
            foreach (var item in LockedItems)
            {
                if (item.Item1 == owner)
                    UnlockItem(item.Item2);
            }
            LockedItems.Clear();
        }

        public void ReleaseLock(object owner, int itemId)
        {
            var position = (Data?.InventoryItems.Where(item => item.InvitemId == itemId).FirstOrDefault()?.Position) ?? throw new Exception("Item not found");
            var lockListIndex = LockedItems.FindIndex(item => item.Item2 == position && owner == item.Item1);
            if (lockListIndex < 0) throw new Exception("Item not found");

            LockedItems.RemoveAt(lockListIndex);
            UnlockItem(position);
        }

        private void UnlockItem(int position)
        {
            ImgSlots[position].Opacity = 1;
        }

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (((FrameworkElement)sender).Tag is ItemDropContext context && ImgSlots[context.ContextId].Source is not null && LockedItems.FindIndex(item => item.Item2 == context.ContextId) == -1)
            {
                var data = new DataObject(typeof(FrameworkElement), sender);
                DragDrop.DoDragDrop((DependencyObject)sender, data, DragDropEffects.Move);
            }
        }

        private void Image_Drop(object sender, DragEventArgs e)
        {
            var target = (FrameworkElement)sender;

            if (e.Data.GetDataPresent(typeof(FrameworkElement)))
            {
                var source = (FrameworkElement)e.Data.GetData(typeof(FrameworkElement));
                var sourceObject = (ItemDropContext)source.Tag;
                var targetObject = (ItemDropContext)target.Tag;

                sourceObject.Owner.ItemDropSourceCallback?.Invoke(sourceObject, targetObject);
                targetObject.Owner.ItemDropTargetCallback?.Invoke(sourceObject, targetObject);
            }
        }
    }
}
