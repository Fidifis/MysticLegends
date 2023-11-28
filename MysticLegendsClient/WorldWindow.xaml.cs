using MysticLegendsClient.CityWindows;
using MysticLegendsShared.Models;
using System.Windows;
using System.Windows.Controls;

namespace MysticLegendsClient
{
    /// <summary>
    /// Interakční logika pro WorldWindow.xaml
    /// </summary>
    public partial class WorldWindow : Window
    {
        private readonly string? filterCity = null;
        public WorldWindow()
        {
            InitializeComponent();
        }

        public WorldWindow(string filterCity):this()
        {
            this.filterCity = filterCity;
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

            foreach (var city in cities)
            {
                if (city.CityName == filterCity)
                    continue;
                AddCityButton(city.CityName);
            }
        }

        private void AddCityButton(string city)
        {
            var btn = new Button
            { 
                Margin = new Thickness(5), 
                FontSize = 20,
                Height = 50,
                Content = city,
            };
            btn.Click += ButtonClick;
            citiesStack.Children.Add(btn);
        }

        private async void ButtonClick(object? sender, RoutedEventArgs e)
        {
            var city = ((Button?)sender)?.Content as string;
            if (city is null)
            {
                MessageBox.Show("Error in reading city name", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (MessageBox.Show($"Do you really want to travel to {city}?", "travel", MessageBoxButton.YesNoCancel) != MessageBoxResult.Yes)
                return;

            int time = -1;
            await ErrorCatcher.TryAsync(async () =>
            {
                time = await ApiCalls.CharacterCall.TravelToCity(city);
            });

            if (time == -1)
                return;

            TravelWindow.DoTravel(time, city);

            DialogResult = true;
            Close();
        }
    }
}
