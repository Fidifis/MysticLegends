using MysticLegendsClient.Controls;
using MysticLegendsClient.Dialogs;
using MysticLegendsClient.NpcWindows;
using MysticLegendsClient.Resources;
using MysticLegendsShared.Models;
using MysticLegendsShared.Utilities;
using System.Windows;

namespace MysticLegendsClient
{
    /// <summary>
    /// Interakční logika pro CityWindow.xaml
    /// </summary>
    public partial class CityWindow : Window
    {
        public enum CloseReason
        {
            Exit,
            SwitchCharacter,
            Logout,
        }

        private static string CityNameToSplash(string cityName) => cityName switch
        {
            "Ayreim" => "/images/Cities/Ayreim.png",
            "Tisling" => "/images/Cities/Tisling.png",
            "Dagos" => "/images/Cities/Dagos.png",
            "Soria" => "/images/Cities/Soria.png",
            _ => throw new NotImplementedException()
        };

        private readonly string cityName;

        private readonly List<SingleInstanceWindow> singletonWindows = new();
        private readonly SingleInstanceWindow<CharacterWindow> characterWindow = new();

        public CityWindow(string cityName): this(cityName, CityNameToSplash(cityName))
        { }

        private CityWindow(string cityName, string splashImage)
        {
            InitializeComponent();
            RefreshCharStats();
            FillButtons(cityName);
            SetSplashImage(splashImage);

            this.cityName = cityName;
            Title = $"Mystic Legends - {cityName} (City)";
            cityNameLabel.Content = cityName;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            GameState.Current.GameEvents.CurrencyUpdateEvent += CurrencyChanged;
            GameState.Current.GameEvents.CharacterUpdateEvent += CharacterStatsChanged;
        }

        private async void FillButtons(string city)
        {
            Npc[]? npcs = null;
            await ErrorCatcher.TryAsync(async () =>
            {
                npcs = await ApiCalls.WorldCall.GetNpcsInCity(city);
            });
            if (npcs is null)
                return;

            foreach (var npc in npcs)
            {
                var window = ShowButton(npc.NpcId, (NpcType)npc.NpcType);
                singletonWindows.Add(window);
            }
        }

        private SingleInstanceWindow ShowButton(int npcId, NpcType button)
        {
            SingleInstanceWindow window;
            switch (button)
            {
                case NpcType.Blacksmith:
                    window = new(typeof(BlacksmithNpc), npcId);
                    AddButton("Blacksmith", Icons.city_blacksmith, (_, _) => { window.Instance.ShowWindow(); });
                    break;
                case NpcType.PotionsCrafter:
                    window = new(typeof(PotionsNpc), npcId);
                    AddButton("Potions", Icons.city_potions, (_, _) => { window.Instance.ShowWindow(); });
                    break;
                case NpcType.Trader:
                    window = new(typeof(TradeWindow), cityName);
                    AddButton("Trade Market", Icons.city_tradeMarket, (_, _) => { window.Instance.ShowWindow(); });
                    break;
                case NpcType.StorageKeeper:
                    window = new(typeof(StorageWindow), cityName);
                    AddButton("Storage", Icons.city_storage, (_, _) => { window.Instance.ShowWindow(); });
                    break;
                case NpcType.RelicTrader:
                    window = new(typeof(RelicTraderWindow), npcId);
                    AddButton("Relic Trader", Icons.city_scout, (_, _) => { window.Instance.ShowWindow(); });
                    break;
                case NpcType.AyreimQueen:
                    window = new(typeof(QueenOfAyreimNpc), npcId);
                    AddButton("Queen of Ayreim", Icons.city_crown, (_, _) => { window.Instance.ShowWindow(); });
                    break;
                default:
                    throw new NotImplementedException();
            }
            return window;
        }

        /*
        protected void ShowButtons(IEnumerable<ButtonType> buttons)
        {
            foreach (var button in buttons)
            {
                switch (button)
                {
                    case ButtonType.Blacksmith:
                        AddButton("Blacksmith", Icons.city_blacksmith, (_, _) => { blacksmithWindow.Instance.ShowWindow(); });
                        break;
                    case ButtonType.Potions:
                        AddButton("Potions", Icons.city_potions, (_, _) => { potionsWindow.Instance.ShowWindow(); });
                        break;
                    case ButtonType.TradeMarket:
                        AddButton("Trade Market", Icons.city_tradeMarket, (_, _) => { });
                        break;
                    case ButtonType.Storage:
                        AddButton("Storage", Icons.city_storage, (_, _) => { });
                        break;
                    case ButtonType.Scout:
                        AddButton("Scout", Icons.city_scout, (_, _) => { });
                        break;
                    case ButtonType.DarkAlley:
                        AddButton("Dark Alley", Icons.city_darkAlley, (_, _) => { });
                        break;
                    case ButtonType.RebelsHideout:
                        AddButton("Rebels Hideout", Icons.city_hideout, (_, _) => { });
                        break;
                }
            }
        }
        */

        protected void AddButton(string title, string icon, RoutedEventHandler onClick)
        {
            var btn = new CityModuleButton { InnerPadding="20 10 20 10", Margin=new Thickness(0, 0, 0, 10), TextGap=50, FontSize=20, UniformSvgSize="50", SvgSource=icon, LabelText=title };
            btn.Click += onClick;
            cityModulesPanel.Children.Add(btn);
        }

        private void CharacterButton_Click(object sender, RoutedEventArgs e)
        {
            characterWindow.Instance.ShowWindow();
        }

        private void CurrencyChanged(object? sender, UpdateEventArgs<int> e)
        {
            currencyLabel.Content = e.Value;
        }

        private async void RefreshCharStats()
        {
            await ErrorCatcher.TryAsync(async () =>
            {
                // TODO: this data is required to fill level and money only, but the following call also requests character items. Make an api call to request only necessary data.
                var character = await ApiCalls.CharacterCall.GetCharacterServerCallAsync(GameState.Current.CharacterName);
                CharacterStatsChanged(this, new(character));
            });
        }

        private void CharacterStatsChanged(object? sender, UpdateEventArgs<Character> e)
        {
            CurrencyChanged(this, new(e.Value.CurrencyGold));
            currentLevel.Content = e.Value.Level;
            nextLevel.Content = e.Value.Level + 1;
            xpProgress.Value = e.Value.Xp;
            xpProgress.Maximum = Leveling.GetXpToLevelUp(e.Value.Level);
        }

        protected void SetSplashImage(string image)
        {
            splashImage.Source = BitmapTools.ImageFromResource(image);
        }

        protected virtual void Window_Closed(object sender, EventArgs e)
        {
            GameState.Current.GameEvents.CurrencyUpdateEvent -= CurrencyChanged;
            GameState.Current.GameEvents.CharacterUpdateEvent -= CharacterStatsChanged;

            singletonWindows.ForEach(window => window.Dispose());
            characterWindow.Dispose();
        }

        private async void Options_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OptionsDialog();
            if (dialog.ShowDialog() != true)
                return;

            switch (dialog.Result)
            {
                case CloseReason.SwitchCharacter:
                    new MainWindow().Show();
                    Close();
                    break;
                case CloseReason.Logout:
                    await ServerConnector.Logout(GameState.Current);
                    new MainWindow().Show();
                    Close();
                    break;
            }
        }

        private void CityModuleButton_Click(object sender, RoutedEventArgs e)
        {
            if (new WorldWindow(cityName).ShowDialog() == true)
            {
                Close();
            }
        }
    }
}
