using MysticLegendsClasses;
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
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            WindowInstance = null;
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var characterData = await (ApiClient.Connection?.GetAsync<CharacterData>("/api/Player/gogomantv/shishka", KeyValuePair.Create("accessToken","lol")) ?? throw new NetworkException("No connection"));
            characterView.FillData(characterData);
        }
    }
}
