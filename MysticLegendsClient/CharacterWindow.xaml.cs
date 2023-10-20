using MysticLegendsClasses;
using System.Collections.Immutable;
using System.Windows;

namespace MysticLegendsClient
{
    /// <summary>
    /// Interakční logika pro CharacterWindow.xaml
    /// </summary>
    public partial class CharacterWindow : Window
    {
        private static CharacterWindow? WindowInstance { get; set; } = null;
        public static void ShowWindow(Window? owner)
        {
            WindowInstance ??= new CharacterWindow() { Owner = owner };
            WindowInstance.Show();
            if (WindowInstance.WindowState == WindowState.Minimized) WindowInstance.WindowState = WindowState.Normal;
            WindowInstance.Activate();
        }

        public static void ShowWindow()
        {
            ShowWindow(null);
        }

        public CharacterWindow()
        {
            InitializeComponent();
            inventoryView.ItemDropCallback = InventoryDrop;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            WindowInstance = null;
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await Refresh();
        }

        private async Task Refresh()
        {
            var characterData = await(ApiClient.Connection?.GetAsync<CharacterData>("/api/Player/gogomantv/shishka") ?? throw new NetworkException("No connection"));
            characterView.FillData(characterData);
            inventoryView.FillData(characterData.Inventory);
        }

        private async void InventoryDrop(InventoryItemContext source, InventoryItemContext target)
        {
            if (source.Owner == target.Owner)
            {
                var parameters = new Dictionary<string, string>
                {
                    ["sourceItem"] = source.Id.ToString(),
                    ["targetItem"] = target.Id.ToString(),
                };
                var newInventory = await (ApiClient.Connection?.PostAsync<InventoryData>("/api/Player/gogomantv/shishka/inventoryswap", parameters.ToImmutableDictionary()) ?? throw new NetworkException("No connection"));
                inventoryView.FillData(newInventory);
            }
        }
    }
}
