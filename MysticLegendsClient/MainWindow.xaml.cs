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
            var gameState = new GameState(connectionUrl);

            if (!await failFastGameState.Connection.HealthCheckAsync())
            {
                MessageBox.Show("Can't connect to server. Try to login again", "Connection failed", MessageBoxButton.OK, MessageBoxImage.Error);
                EnterLogin();
                return;
            }

            GameState.MakeGameStateCurrent(gameState);
            if (await ServerConnector.Authenticate(gameState))
            {
                await EnterCharacterSelect(gameState);
            }
            else
            {
                EnterLogin();
            }
        }

        private void EnterGame()
        {
            new AyreimCity().Show();
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
                EnterGame();
            }
            Close();
        }
    }
}
