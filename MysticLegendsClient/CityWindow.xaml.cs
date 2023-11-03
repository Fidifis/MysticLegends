using MysticLegendsClient.Controls;
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

        public CityWindow()
        {
            InitializeComponent();
            _=RefreshCurrencyAsync();
            GameState.Current.CurrencyUpdateEvent += CurrencyChanged;
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
                        AddButton("Blacksmith", Icons.city_blacksmith);
                        break;
                    case ButtonType.Potions:
                        AddButton("Potions", Icons.city_potions);
                        break;
                    case ButtonType.TradeMarket:
                        AddButton("Trade Market", Icons.city_tradeMarket);
                        break;
                    case ButtonType.Storage:
                        AddButton("Storage", Icons.city_storage);
                        break;
                    case ButtonType.Scout:
                        AddButton("Scout", Icons.city_scout);
                        break;
                    case ButtonType.DarkAlley:
                        AddButton("Dark Alley", Icons.city_darkAlley);
                        break;
                    case ButtonType.RebelsHideout:
                        AddButton("Rebels Hideout", Icons.city_hideout);
                        break;
                }
            }
        }

        protected void AddButton(string title, string icon)
        {
            cityModulesPanel.Children.Add(new CityModuleButton { InnerPadding="20 10 20 10", Margin=new Thickness(0, 0, 0, 10), TextGap=50, FontSize=20, UniformSvgSize="50", SvgSource=icon, LabelText=title });
        }

        private void CharacterButton_Click(object sender, RoutedEventArgs e)
        {
            characterWindow.Instance.ShowWindow();
        }

        private void CurrencyChanged(object? sender, CurrencyUpdateEventArgs e)
        {
            currencyLabel.Content = e.Value;
        }

        private async Task RefreshCurrencyAsync()
        {
            var currency = await GameState.Current.Connection.GetAsync<int>("/api/Character/zmrdus/currency");
            CurrencyChanged(this, new CurrencyUpdateEventArgs(currency));
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            characterWindow.Dispose();
        }
    }
}
