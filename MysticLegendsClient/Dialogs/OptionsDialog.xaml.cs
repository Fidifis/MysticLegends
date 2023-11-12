using System.Windows;

namespace MysticLegendsClient.Dialogs
{
    /// <summary>
    /// Interakční logika pro Menu.xaml
    /// </summary>
    public partial class OptionsDialog : Window
    {
        public CityWindow.CloseReason Result { get; private set; }

        public OptionsDialog()
        {
            InitializeComponent();
        }

        private void SwitchChar_Click(object sender, RoutedEventArgs e)
        {
            Result = CityWindow.CloseReason.SwitchCharacter;
            DialogResult = true;
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            Result = CityWindow.CloseReason.Logout;
            DialogResult = true;
        }
    }
}
