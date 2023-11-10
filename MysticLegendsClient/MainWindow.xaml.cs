using MysticLegendsClient.CityWindows;
using MysticLegendsShared.Models;
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
            splashImage.Source = BitmapTools.ImageFromResource(resource);
        }


        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (!await GameState.Current.Connection.HealthCheckAsync())
            {
                MessageBox.Show("Can't connect to server", "Connection failed", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (await Authenticate())
            {
                EnterGame();
            }

            remember.IsChecked = GameState.Current.TokenStore.GetRefreshToken() is not null;
            // TODO: Fill username

            SwitchLoginInterface(true);
        }

        private async void Login_Click(object sender, RoutedEventArgs e)
        {
            if (!await GameState.Current.Connection.HealthCheckAsync())
            {
                MessageBox.Show("Can't connect to server", "Connection failed", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (username.Text == "" || password.Password == "")
            {
                MessageBox.Show("Please enter username and password");
                return;
            }

            SwitchLoginInterface(false);

            try
            {
                var refreshToken = await ApiCalls.AuthCall.LoginServerCallAsync(username.Text, password.Password);
                var accessToken = await ApiCalls.AuthCall.TokenServerCallAsync(refreshToken);

                if (remember.IsChecked == true)
                    GameState.Current.TokenStore.SaveRefreshToken(refreshToken);

                GameState.Current.TokenStore.AccessToken = accessToken;
                EnterGame();
            }
            catch (Exception)
            {
                MessageBox.Show("Failed to login");
                SwitchLoginInterface(true);
                return;
            }
        }

        private async Task<bool> Authenticate()
        {
            var refreshToken = GameState.Current.TokenStore.GetRefreshToken();
            if (refreshToken is null)
                return false;

            try
            {
                var accressToken = await ApiCalls.AuthCall.TokenServerCallAsync(refreshToken);
                GameState.Current.TokenStore.AccessToken = accressToken;
                return true;
            }
            catch (Exception) { }
            
            return false;
        }

        private void EnterGame()
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

        private void SwitchLoginInterface(bool isEnabled)
        {
            loginButton.IsEnabled = isEnabled;
            registerButton.IsEnabled = isEnabled;
            loggingInLabel.Visibility = isEnabled ? Visibility.Collapsed : Visibility.Visible;
        }
    }
}
