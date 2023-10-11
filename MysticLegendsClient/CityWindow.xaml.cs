using System.Windows;

namespace MysticLegendsClient
{
    /// <summary>
    /// Interakční logika pro CityWindow.xaml
    /// </summary>
    public partial class CityWindow : Window
    {
        public CityWindow()
        {
            InitializeComponent();
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e) =>
            topBarGrid.ColumnDefinitions[0].Width = e.NewSize.Width < 700 ? new GridLength(0) : new GridLength(1, GridUnitType.Star);
    }
}
