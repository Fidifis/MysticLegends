using MysticLegendsClient.CityWindows;
using System.Windows;

namespace MysticLegendsClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        string[] SplashImages = { "/images/Graphics/LoginScreen.png", "/images/Graphics/LoginScreen2.png", "/images/Graphics/LoginScreen3.png" };
        public LoginWindow()
        {
            InitializeComponent();
            serverSelect.ItemsSource = Enum.GetValues(typeof(ServerConnector.ServerConncetionType));
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
            var connectionTypeString = await GameState.Current.ConfigStore.ReadAsync("connectionType") ?? ServerConnector.ServerConncetionType.OfficialServers.ToString();
            var connectionType = Enum.Parse<ServerConnector.ServerConncetionType>(connectionTypeString);
            serverSelect.SelectedItem = connectionType;

            if (connectionType == ServerConnector.ServerConncetionType.Custom)
            {
                customServerTxt.Text = await GameState.Current.ConfigStore.ReadAsync("customConnectionAddress");
            }

            remember.IsChecked = await GameState.Current.TokenStore.ReadRefreshTokenAsync(GameState.Current.Connection.Host) is not null;
            username.Text = await GameState.Current.TokenStore.ReadUserNameAsync(GameState.Current.Connection.Host);

            SwitchLoginInterface(true);
        }

        private async void Login_Click(object sender, RoutedEventArgs e)
        {
            var connectionType = (ServerConnector.ServerConncetionType)serverSelect.SelectedItem;
            var gameState = new GameState(ServerConnector.ConnectionTypeToUrl(connectionType, customServerTxt.Text));

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
                await ServerConnector.Login(username.Text, password.Password, remember.IsChecked == true, gameState);

                await gameState.ConfigStore.WriteAsync("connectionType", connectionType.ToString());
                await gameState.ConfigStore.WriteAsync("customConnectionAddress", customServerTxt.Text == "" ? null : customServerTxt.Text);

                GameState.MakeGameStateCurrent(gameState);
                DialogResult = true;
                Close();
            }
            catch (Exception)
            {
                MessageBox.Show("Failed to login");
                SwitchLoginInterface(true);
                return;
            }
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
