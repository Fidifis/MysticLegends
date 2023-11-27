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
        public WorldWindow()
        {
            InitializeComponent();
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
            new TravelWindow(10, () => {
                new AyreimCity().Show();
            }).Show();

            DialogResult = true;
            Close();
        }
    }
}
