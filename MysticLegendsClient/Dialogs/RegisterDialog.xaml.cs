using System.Windows;

namespace MysticLegendsClient.Dialogs;

/// <summary>
/// Interaction logic for RegisterDialog.xaml
/// </summary>
public partial class RegisterDialog : Window
{
    public string Username { get; private set; } = "";
    public string Password { get; private set; } = "";

    public RegisterDialog(string username = "", string password = "")
    {
        InitializeComponent();
        usernameBox.Text = username;
        passwordBox.Text = password;
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        if (usernameBox.Text.Trim() == "" || passwordBox.Text.Trim() == "" || confirmPasswordBox.Text.Trim() == "")
        {
            MessageBox.Show("Please fill all fields");
            return;
        }
        if (passwordBox.Text != confirmPasswordBox.Text)
        {
            MessageBox.Show("Passwords don't match");
            return;
        }
        
        if (usernameBox.Text.Contains(' '))
        {
            MessageBox.Show("Username cann't contain spaces and special characters");
            return;
        }

        Username = usernameBox.Text.Trim();
        Password = passwordBox.Text;
        DialogResult = true;
        Close();
    }
}
