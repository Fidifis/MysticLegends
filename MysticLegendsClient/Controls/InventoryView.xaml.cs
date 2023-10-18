using MysticLegendsClasses;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using MysticLegendsClient.Resources;

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

        private int ItemCount { get => ImgSlots.Count(item => item.Source is not null); }
        private int Capacity { get => ImgSlots.Count; }
        private string CapacityCounter { get => $"{ItemCount}/{Capacity}"; }

        private List<Image> ImgSlots = new();

        private UIElement CreateSlot(out Image image)
        {
            image = new Image
            {
                Width = 75,
                Height = 75,
            };
            return new Border
            {
                Margin = new Thickness(5),
                BorderThickness = new Thickness(3),
                BorderBrush = Brushes.Black,
                Child = image
            };
        }

        private void RemoveSlotRange(int index, int count)
        {
            inventoryPanel.Children.RemoveRange(index, count);
            ImgSlots.RemoveRange(index, count);
        }

        private void UpdateSlots(int count)
        {
            Debug.Assert(ImgSlots.Count == inventoryPanel.Children.Count, "list counts differ");
            if (count > ImgSlots.Count)
                for (int i = ImgSlots.Count; i < count; i++)
                {
                    inventoryPanel.Children.Add(CreateSlot(out Image image));
                    ImgSlots.Add(image);
                }
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
            for (int i = 0; i < inventoryData.Items.Count; i++)
            {
                if (inventoryData.Items[i] is null)
                    ImgSlots[i].Source = null;
                else
                    ImgSlots[i].Source = BitmapTools.FromResource(Items.ResourceManager.GetString(inventoryData.Items[i]!.Value.Icon)!);
            }

            UpdateCapacityCounter();
        }
    }
}
