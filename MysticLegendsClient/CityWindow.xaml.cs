using MysticLegendsClient.Controls;
using System.Windows;

namespace MysticLegendsClient
{
    /// <summary>
    /// Interakční logika pro CityWindow.xaml
    /// </summary>
    public abstract partial class CityWindow : Window
    {
        public CityWindow()
        {
            InitializeComponent();
        }

        public CityWindow(string title): this()
        {
            Title = $"Mystic Legends - {title} (City)";
            cityNameLabel.Content = title;
        }

        private void CharacterButton_Click(object sender, RoutedEventArgs e)
        {
            CharacterWindow.ShowWindow(this);
        }
    }
}
