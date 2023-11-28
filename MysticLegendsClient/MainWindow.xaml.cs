using MysticLegendsClient.CityWindows;
using MysticLegendsShared.Utilities;
using System.Windows;

namespace MysticLegendsClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var customConnection = await GameState.Current.ConfigStore.ReadAsync("customConnectionAddress");
            var connectionTypeString = await GameState.Current.ConfigStore.ReadAsync("connectionType") ?? ServerConnector.ServerConncetionType.OfficialServers.ToString();
            var connectionType = Enum.Parse<ServerConnector.ServerConncetionType>(connectionTypeString);
            var connectionUrl = ServerConnector.ConnectionTypeToUrl(connectionType, customConnection);

            using var failFastGameState = new GameState(connectionUrl, TimeSpan.FromSeconds(3));

            if (!await failFastGameState.Connection.HealthCheckAsync())
            {
                MessageBox.Show("Can't connect to server. Try to login again", "Connection failed", MessageBoxButton.OK, MessageBoxImage.Error);
                EnterLogin();
                return;
            }

            if (GameState.Current.TokenStore.AccessToken is not null)
            {
                await EnterCharacterSelect(GameState.Current);
                return;
            }

            var gameState = new GameState(connectionUrl);
            GameState.MakeGameStateCurrent(gameState);
            if (await ServerConnector.Authenticate(gameState))
            {
                await EnterCharacterSelect(gameState);
                return;
            }

            EnterLogin();
        }

        private async Task EnterGame()
        {
            await ErrorCatcher.TryAsync(async () =>
            {
                var data = await ApiCalls.CharacterCall.GetCharacterCityCallAsync(GameState.Current.CharacterName);
                var city = data["city"];

                if (data.TryGetValue("travel", out string? travelTime))
                    TravelWindow.DoTravel(int.Parse(travelTime), city);
                else
                    CityWindow.MakeCity(city).Show();
            });
            Close();
        }

        private async void EnterLogin()
        {
            Hide();
            var loginWindow = new LoginWindow();
            if (loginWindow.ShowDialog() == true)
                await EnterCharacterSelect(GameState.Current);
            Close();
        }

        private async Task EnterCharacterSelect(GameState gameState)
        {
            IEnumerable<CharacterSelect.CharacterDisplayData>? data = null;
            await ErrorCatcher.TryAsync(async () =>
            {
                data = (await ApiCalls.UserCall.GetUserCharactersServerCallAsync(gameState.Username))
                    .Select(character => new CharacterSelect.CharacterDisplayData(character.CharacterName, character.Level, (CharacterClass)character.CharacterClass));
            });
            if (data is null)
            {
                EnterLogin();
                return;
            }

            Hide();
            var charSelect = new CharacterSelect(gameState.Username, data);
            if (charSelect.ShowDialog() == true)
            {
                gameState.CharacterName = charSelect.ResultCharacterName!;
                await EnterGame();
            }
            Close();
        }
    }
}
