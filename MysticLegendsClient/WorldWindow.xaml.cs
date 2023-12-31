﻿using MysticLegendsClient.Dialogs;
using MysticLegendsShared.Models;
using System.Windows;
using System.Windows.Controls;

namespace MysticLegendsClient
{
    /// <summary>
    /// Interakční logika pro WorldWindow.xaml
    /// </summary>
    public partial class WorldWindow : Window, ISingleInstanceWindow
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

        public void ShowWindow()
        {
            SingleInstanceWindow.CommonShowWindowTasks(this);
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            City[]? cities = null;
            Area[]? areas = null;
            await ErrorCatcher.TryAsync(async () =>
            {
                var citiesTask = ApiCalls.WorldCall.GetCitiesAsync();
                var areasTask = ApiCalls.WorldCall.GetAreasAsync();
                cities = await citiesTask;
                areas = await areasTask;
            });

            if (cities is null || areas is null)
                return;

            foreach (var city in cities)
            {
                if (city.CityName == filterCity)
                    continue;
                AddCityButton(city.CityName);
            }

            foreach (var area in areas)
            {
                if (area.AreaName == filterCity)
                    continue;
                AddAreaButton(area.AreaName);
            }
        }

        private void AddCityButton(string city)
        {
            var btn = AddButton(city);
            btn.Click += ButtonClick;
            citiesStack.Children.Add(btn);
        }

        private void AddAreaButton(string area)
        {
            var btn = AddButton(area);
            btn.Click += AreaButtonClick;
            areasStack.Children.Add(btn);
        }

        private static Button AddButton(string content) => new Button
        {
            Margin = new Thickness(5),
            FontSize = 20,
            Height = 50,
            Content = content,
        };

        private async void ButtonClick(object? sender, RoutedEventArgs e)
        {
            var city = ((Button?)sender)?.Content as string;
            if (city is null)
            {
                MessageBox.Show("Error in reading city name", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (MessageBox.Show($"Do you really want to travel to {city}?", "travel", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                return;

            int time = -1;
            await ErrorCatcher.TryAsync(async () =>
            {
                time = await ApiCalls.CharacterCall.TravelToCity(city);
            });

            if (time == -1)
                return;

            TravelWindow.DoTravelToCity(time, city);

            DialogResult = true;
            Close();
        }

        private async void AreaButtonClick(object? sender, RoutedEventArgs e)
        {
            var area = ((Button?)sender)?.Content as string;
            if (area is null)
            {
                MessageBox.Show("Error in reading area name", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            Mob[] mobs;
            try
            {
                mobs = await ApiCalls.WorldCall.GetMobsInArea(area);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var dialog = new MobsInAreaDialog(mobs);

            if (dialog.ShowDialog() != true)
                return;

            await ErrorCatcher.TryAsync(async () =>
            {
                var fightResult = await ApiCalls.CharacterCall.FightCall(dialog.SelectedMob!.MobId);
                TravelWindow.DoTravelToArea(
                    fightResult.TravelTime, fightResult.ReturnCity,
                    new(fightResult.Win, dialog.SelectedMob!, fightResult.DropedItems ?? Array.Empty<InventoryItem>()));
            });

            DialogResult = true;
            Close();
        }
    }
}
