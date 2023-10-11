using MysticLegendsClient.CityWindows;
using System.Windows;
using System.Windows.Media.Imaging;

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
            BitmapImage bitmapImage = new BitmapImage(new Uri($"pack://application:,,,{resource}", UriKind.Absolute));
            splashImage.Source = bitmapImage;
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
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
