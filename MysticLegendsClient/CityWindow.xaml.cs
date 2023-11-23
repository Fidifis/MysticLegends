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
    public abstract partial class CityWindow : Window
    {
        public enum CloseReason
        {
            Exit,
            SwitchCharacter,
            Logout,
        }

        protected enum ButtonType
        {
            Blacksmith,
            Potions,
            TradeMarket,
            Storage,
            Scout,
            DarkAlley,
            RebelsHideout,
        }

        private readonly SingleInstanceWindow<CharacterWindow> characterWindow = new();
        private readonly SingleInstanceWindow blacksmithWindow = new(typeof(BlacksmithNpc), 2);
        private readonly SingleInstanceWindow potionsWindow = new(typeof (PotionsNpc), 1);
        private readonly SingleInstanceWindow darkAlleyWindow = new(typeof(DarkAlleyNpc), 3);

        public CityWindow()
        {
            InitializeComponent();
            RefreshCharStats();
        }

        public CityWindow(string title): this()
        {
            Title = $"Mystic Legends - {title} (City)";
            cityNameLabel.Content = title;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            GameState.Current.GameEvents.CurrencyUpdateEvent += CurrencyChanged;
            GameState.Current.GameEvents.CharacterUpdateEvent += CharacterStatsChanged;
        }

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
                        AddButton("Dark Alley", Icons.city_darkAlley, (_, _) => { darkAlleyWindow.Instance.ShowWindow(); });
                        break;
                    case ButtonType.RebelsHideout:
                        AddButton("Rebels Hideout", Icons.city_hideout, (_, _) => { });
                        break;
                }
            }
        }

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

            characterWindow.Dispose();
            blacksmithWindow.Dispose();
            potionsWindow.Dispose();
            darkAlleyWindow.Dispose();
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
    }
}
