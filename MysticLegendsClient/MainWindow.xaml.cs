﻿using MysticLegendsClient.CityWindows;
using System.Windows;

namespace MysticLegendsClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string[] SplashImages = { "/images/Graphics/LoginScreen.png", "/images/Graphics/LoginScreen2.png", "/images/Graphics/LoginScreen3.png" };
        public MainWindow()
        {
            InitializeComponent();
            ChangeSplashImage(ChooseRandom(SplashImages));
        }

        private static string ChooseRandom(string[] list)
        {
            Random random = new();
            int index = random.Next(list.Length);
            return list[index];
        }

        private void ChangeSplashImage(string resource)
        {
            splashImage.Source = BitmapTools.FromResource(resource);
        }

        private async void Login_Click(object sender, RoutedEventArgs e)
        {
            if (!await GameState.Current.Connection.HealthCheckAsync())
            {
                MessageBox.Show("Can't connect to server", "Connection failed", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (remember.IsChecked == true)
                GameState.Current.TokenStore.SaveRefreshToken("random");

            new AyreimCity().Show();
            Close();
        }

        private void ServerSelect_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (customServerControls is null) return;
            customServerControls.Visibility =
                serverSelect.SelectedIndex == serverSelect.Items.Count - 1 ?
                Visibility.Visible : Visibility.Collapsed;
        }
    }
}
