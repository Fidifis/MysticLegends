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

        protected void AddButton(string title, string icon)
        {
            cityModulesPanel.Children.Add(new CityModuleButton { InnerPadding="20 10 20 10", Margin=new Thickness(0, 0, 0, 10), TextGap=50, FontSize=20, UniformSvgSize="50", SvgSource=icon, LabelText=title });
        }

        private void CharacterButton_Click(object sender, RoutedEventArgs e)
        {
            CharacterWindow.ShowWindow();
        }
    }
}
