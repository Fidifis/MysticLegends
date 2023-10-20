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

        private UIElement CreateSlot(out Image image)
        {
            image = new Image
            {
                Width = 75,
                Height = 75,
            };
            image.MouseLeftButtonDown += Image_MouseLeftButtonDown;
            image.Drop += Image_Drop;
            return new Border
            {
                Margin = new Thickness(5),
                BorderThickness = new Thickness(3),
                BorderBrush = Brushes.Black,
                AllowDrop = true,
                Child = image,
            };
        }

        private void RemoveSlotRange(int index, int count)
        {
            inventoryPanel.Children.RemoveRange(index, count);
            ImgSlots.RemoveRange(index, count);
        }

        private void AddToSlot(UIElement element, Image image)
        {
            inventoryPanel.Children.Add(element);
            ImgSlots.Add(image);
            image.Tag = new InventoryItemContext(this, ImgSlots.Count - 1);
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
            var data = new DataObject(typeof(Image), sender);
            DragDrop.DoDragDrop((DependencyObject)sender, data, DragDropEffects.Move);
        }

        private void Image_Drop(object sender, DragEventArgs e)
        {
            Image targetImage = (Image)sender;

            if (e.Data.GetDataPresent(typeof(Image)))
            {
                Image sourceImage = (Image)e.Data.GetData(typeof(Image));
                ItemDropCallback?.Invoke((InventoryItemContext)sourceImage.Tag, (InventoryItemContext)targetImage.Tag);
            }
        }
    }
}
