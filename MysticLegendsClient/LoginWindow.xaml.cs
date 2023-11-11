using MysticLegendsClient.CityWindows;
using System.Windows;

namespace MysticLegendsClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        private enum ServerConncetionType
        {
            OfficialServers,
            Localhost,
            Custom
        }

        string[] SplashImages = { "/images/Graphics/LoginScreen.png", "/images/Graphics/LoginScreen2.png", "/images/Graphics/LoginScreen3.png" };
        public LoginWindow()
        {
            InitializeComponent();
            serverSelect.ItemsSource = Enum.GetValues(typeof(ServerConncetionType));
            ChangeSplashImage(ChooseRandom(SplashImages));
        }

        private static string ChooseRandom(string[] list)
        {
            int index = Random.Shared.Next(list.Length);
            return list[index];
        }

        private void ChangeSplashImage(string resource)
        {
            splashImage.Source = BitmapTools.ImageFromResource(resource);
        }


        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var customConnection = await GameState.Current.ConfigStore.ReadAsync("customConnectionAddress");
            var connectionTypeString = await GameState.Current.ConfigStore.ReadAsync("connectionType") ?? ServerConncetionType.OfficialServers.ToString();
            var connectionType = Enum.Parse<ServerConncetionType>(connectionTypeString);
            var connectionUrl = ConnectionTypeToUrl(connectionType, customConnection);
            serverSelect.SelectedItem = connectionType;

            using var failFastGameState = new GameState(connectionUrl, TimeSpan.FromSeconds(3));

            if (!await failFastGameState.Connection.HealthCheckAsync())
            {
                //MessageBox.Show("Can't connect to server", "Connection failed", MessageBoxButton.OK, MessageBoxImage.Error);
                SwitchLoginInterface(true);
                return;
            }

            var gameState = new GameState(connectionUrl);
            if (await Authenticate(gameState))
            {
                GameState.MakeGameStateCurrent(gameState);
                EnterGame();
            }

            remember.IsChecked = await failFastGameState.TokenStore.ReadRefreshTokenAsync(failFastGameState.Connection.Host) is not null;
            username.Text = await failFastGameState.TokenStore.ReadUserNameAsync(failFastGameState.Connection.Host);

            SwitchLoginInterface(true);
        }

        private async void Login_Click(object sender, RoutedEventArgs e)
        {
            var connectionType = (ServerConncetionType)serverSelect.SelectedItem;
            var gameState = new GameState(ConnectionTypeToUrl(connectionType));

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
                    await gameState.TokenStore.SaveRefreshToken(refreshToken, gameState.Connection.Host);
                    await gameState.TokenStore.SaveUsername(username.Text, gameState.Connection.Host);
                }

                gameState.ChangeAccessToken(accessToken);
                await gameState.ConfigStore.WriteAsync("connectionType", connectionType.ToString());
                await gameState.ConfigStore.WriteAsync("customConnectionAddress", customServerTxt.Text == "" ? null : customServerTxt.Text);

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
            var refreshToken = await gameState.TokenStore.ReadRefreshTokenAsync(gameState.Connection.Host);
            if (refreshToken is null)
                return false;

            try
            {
                var accessToken = await ApiCalls.AuthCall.TokenServerCallAsync(refreshToken, gameState);
                gameState.ChangeAccessToken(accessToken);
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

        private string ConnectionTypeToUrl(ServerConncetionType connectionType, string? customSubstitute = null)
        {
            return connectionType switch
            {
                ServerConncetionType.OfficialServers => GameState.OfficialServersUrl,
                ServerConncetionType.Localhost => "http://localhost:5281",
                ServerConncetionType.Custom => customSubstitute ?? customServerTxt.Text,
                _ => ""
            };
        }
    }
}
