using MysticLegendsClient.Dialogs;
using System.Windows;

namespace MysticLegendsClient;

/// <summary>
/// Interakční logika pro TravelWindow.xaml
/// </summary>
public partial class TravelWindow : Window
{
    public static void DoTravelToCity(int seconds, string city)
    {
        new TravelWindow(seconds, () => {
            new CityWindow(city).Show();
        }).Show();
    }

    public static void DoTravelToArea(int seconds, string city, FightResultDialog.DisplayData data)
    {
        new TravelWindow(seconds, () =>
        {
            var cityWindow = new CityWindow(city);
            cityWindow.Show();

            new FightResultDialog(data) { Owner = cityWindow }.Show();
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
