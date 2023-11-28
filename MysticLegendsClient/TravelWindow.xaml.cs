using System.Windows;

namespace MysticLegendsClient;

/// <summary>
/// Interakční logika pro TravelWindow.xaml
/// </summary>
public partial class TravelWindow : Window
{
    public static void DoTravel(int seconds, string city)
    {
        new TravelWindow(seconds, () => {
            CityWindow.MakeCity(city).Show();
        }).Show();
    }

    private readonly Action onCompletion;
    public TravelWindow(int seconds, Action onCompletion)
    {
        InitializeComponent();
        progressBar.Maximum = seconds;
        this.onCompletion = onCompletion;
    }

    private async void Window_Loaded(object sender, RoutedEventArgs e)
    {
        while (progressBar.Value < progressBar.Maximum)
        {
            progressBar.Value += 0.01;
            timer.Content = ((int)(progressBar.Maximum - progressBar.Value) + 1).ToString();
            await Task.Delay(10);
        }
        onCompletion.Invoke();
        Close();
    }
}
