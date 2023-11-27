using MysticLegendsShared.Models;
using System.Windows;

namespace MysticLegendsClient
{
    /// <summary>
    /// Interakční logika pro WorldWindow.xaml
    /// </summary>
    public partial class WorldWindow : Window, ISingleInstanceWindow
    {
        public WorldWindow()
        {
            InitializeComponent();
        }

        public void ShowWindow()
        {
            SingleInstanceWindow.CommonShowWindowTasks(this);
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            City[]? cities = null;
            await ErrorCatcher.TryAsync(async () =>
            {
                cities = await ApiCalls.WorldCall.GetCitiesAsync();
            });

            if (cities is null)
                return;

            // TODO add
        }
    }
}
