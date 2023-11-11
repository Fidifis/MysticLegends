using System.Windows;

namespace MysticLegendsClient.Dialogs
{
    /// <summary>
    /// Interakční logika pro EnterTextDialog.xaml
    /// </summary>
    public partial class EnterTextDialog : Window
    {
        public string EnteredText => mainTextBox.Text;

        public EnterTextDialog(string headline, string title = "")
        {
            InitializeComponent();
            headLine.Content = headline;
            Title = title;
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            if (mainTextBox.Text == "")
                return;
            DialogResult = true;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
