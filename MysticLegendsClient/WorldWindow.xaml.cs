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
                Content = city, // in future should display city name as human readable string - Name with spaces
                Tag = city, // tag holds a city's db primary key - name in PsacalCase
            };
            btn.Click += ButtonClick;
            citiesStack.Children.Add(btn);
        }

        private void ButtonClick(object? sender, RoutedEventArgs e)
        {
            var city = ((FrameworkElement?)sender)?.Tag as string;
            if (MessageBox.Show($"Do you really want to travel to {city}?", "travel", MessageBoxButton.YesNoCancel) != MessageBoxResult.Yes)
                return;

            new TravelWindow(10, () => {
                new AyreimCity().Show();
            }).Show();

            DialogResult = true;
            Close();
        }
    }
}
