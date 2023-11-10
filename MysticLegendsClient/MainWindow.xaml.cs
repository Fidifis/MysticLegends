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
        private enum ServerConncetionType
        {
            OfficialServers,
            Localhost,
            Custom
        }

        string[] SplashImages = { "/images/Graphics/LoginScreen.png", "/images/Graphics/LoginScreen2.png", "/images/Graphics/LoginScreen3.png" };
        public MainWindow()
        {
            InitializeComponent();
            serverSelect.ItemsSource = Enum.GetValues(typeof (ServerConncetionType));
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
            using var failFastGameState = new GameState(GameState.OfficialServersUrl, TimeSpan.FromSeconds(3));

            if (!await failFastGameState.Connection.HealthCheckAsync())
            {
                //MessageBox.Show("Can't connect to server", "Connection failed", MessageBoxButton.OK, MessageBoxImage.Error);
                SwitchLoginInterface(true);
                return;
            }

            if (await Authenticate(failFastGameState))
            {
                EnterGame();
            }

            remember.IsChecked = await failFastGameState.TokenStore.ReadRefreshTokenAsync() is not null;
            username.Text = await failFastGameState.TokenStore.ReadUserNameAsync();

            SwitchLoginInterface(true);
        }

        private async void Login_Click(object sender, RoutedEventArgs e)
        {
            var gameState = (ServerConncetionType)serverSelect.SelectedItem switch
            {
                ServerConncetionType.OfficialServers => new GameState(),
                ServerConncetionType.Localhost => new GameState("http://localhost:5281"),
                ServerConncetionType.Custom => new GameState(customServerTxt.Text),
                _ => new GameState()
            };

            if (username.Text == "" || password.Password == "")
            {
                MessageBox.Show("Please enter username and password");
                return;
            }

            SwitchLoginInterface(false);

            if (!await gameState.Connection.HealthCheckAsync())
            {
                MessageBox.Show("Can't connect to server", "Connection failed", MessageBoxButton.OK, MessageBoxImage.Error);
                SwitchLoginInterface(true);
                return;
            }

            try
            {
                var refreshToken = await ApiCalls.AuthCall.LoginServerCallAsync(username.Text, password.Password, gameState);
                var accessToken = await ApiCalls.AuthCall.TokenServerCallAsync(refreshToken, gameState);

                if (remember.IsChecked == true)
                {
                    await gameState.TokenStore.SaveRefreshToken(refreshToken);
                    await gameState.TokenStore.SaveUsername(username.Text);
                }

                gameState.TokenStore.AccessToken = accessToken;

                GameState.MakeGameStateCurrent(gameState);
                EnterGame();
            }
            catch (Exception)
            {
                MessageBox.Show("Failed to login");
                SwitchLoginInterface(true);
                return;
            }
        }

        private async Task<bool> Authenticate(GameState gameState)
        {
            var refreshToken = await gameState.TokenStore.ReadRefreshTokenAsync();
            if (refreshToken is null)
                return false;

            try
            {
                var accressToken = await ApiCalls.AuthCall.TokenServerCallAsync(refreshToken);
                gameState.TokenStore.AccessToken = accressToken;
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
