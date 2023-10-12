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
    }
}
