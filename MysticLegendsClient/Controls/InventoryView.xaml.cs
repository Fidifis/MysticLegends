using MysticLegendsClasses;
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
    public partial class InventoryView : UserControl
    {
        public InventoryView()
        {
            InitializeComponent();
            DataContext = this;
        }

        public delegate void ItemDrop(InventoryItemContext source, InventoryItemContext target);
        public ItemDrop? ItemDropCallback { get; set; }

        private int ItemCount { get => ImgSlots.Count(item => item.Source is not null); }
        private int Capacity { get => ImgSlots.Count; }
        private string CapacityCounter { get => $"{ItemCount}/{Capacity}"; }

        private readonly List<Image> ImgSlots = new();

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
            element.Tag = new InventoryItemContext(this, ImgSlots.Count - 1);
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

        public void FillData(InventoryData inventoryData)
        {
            if (inventoryData.Items is null)
            {
                Debug.Assert(false);
                return;
            }

            UpdateSlots((int)inventoryData.Capacity);
            for (int i = 0; i < ImgSlots.Count; i++)
                ImgSlots[i].Source = null;

            foreach (var item in inventoryData.Items)
            {
                ImgSlots[(int)item.InventoryPosition].Source = BitmapTools.FromResource(Items.ResourceManager.GetString(item.Icon)!);
            }

            UpdateCapacityCounter();
        }

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (((FrameworkElement)sender).Tag is InventoryItemContext context && ImgSlots[context.Id].Source is not null)
            {
                var data = new DataObject(typeof(FrameworkElement), sender);
                DragDrop.DoDragDrop((DependencyObject)sender, data, DragDropEffects.Copy);
            }
        }

        private void Image_Drop(object sender, DragEventArgs e)
        {
            var targetImage = (FrameworkElement)sender;

            if (e.Data.GetDataPresent(typeof(FrameworkElement)))
            {
                var sourceImage = (FrameworkElement)e.Data.GetData(typeof(FrameworkElement));
                ItemDropCallback?.Invoke((InventoryItemContext)sourceImage.Tag, (InventoryItemContext)targetImage.Tag);
            }
        }
    }
}
