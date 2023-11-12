using MysticLegendsClient.Controls;
using MysticLegendsClient.NpcWindows;
using MysticLegendsClient.Resources;
using System.Windows;

namespace MysticLegendsClient
{
    /// <summary>
    /// Interakční logika pro CityWindow.xaml
    /// </summary>
    public abstract partial class CityWindow : Window
    {
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
        private readonly SingleInstanceWindow potionsWindow = new(typeof (PotionsNpc), 1);

        public CityWindow()
        {
            InitializeComponent();
            RefreshCurrency();
            GameState.Current.GameEvents.CurrencyUpdateEvent += CurrencyChanged;
        }

        public CityWindow(string title): this()
        {
            Title = $"Mystic Legends - {title} (City)";
            cityNameLabel.Content = title;
        }

        protected void ShowButtons(IEnumerable<ButtonType> buttons)
        {
            foreach (var button in buttons)
            {
                switch (button)
                {
                    case ButtonType.Blacksmith:
                        AddButton("Blacksmith", Icons.city_blacksmith, (_, _) => { });
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

        private async void RefreshCurrency()
        {
            await ErrorCatcher.TryAsync(async () =>
            {
                var currency = await ApiCalls.CharacterCall.GetCharacterCurrencyCallAsync(GameState.Current.CharacterName);
                CurrencyChanged(this, new(currency));
            });
        }

        protected virtual void Window_Closed(object sender, EventArgs e)
        {
            characterWindow.Dispose();
            potionsWindow.Dispose();
        }
    }
}
