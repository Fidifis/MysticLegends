using System.Windows;
using System.Windows.Input;

namespace MysticLegendsClient
{
    /// <summary>
    /// Interakční logika pro TradeWindow.xaml
    /// </summary>
    public partial class TradeWindow : Window, ISingleInstanceWindow
    {
        private readonly string cityName;

        public TradeWindow(string cityName)
        {
            InitializeComponent();
            this.cityName = cityName;
        }

        public void ShowWindow()
        {
            SingleInstanceWindow.CommonShowWindowTasks(this);
        }

        private void ItemSlot_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void ItemSlot_Drop(object sender, DragEventArgs e)
        {

        }
    }
}
