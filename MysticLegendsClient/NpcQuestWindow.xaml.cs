using System.Windows;

namespace MysticLegendsClient;

/// <summary>
/// Interaction logic for NpcQuestWindow.xaml
/// </summary>
public abstract partial class NpcQuestWindow : NpcWindow
{
    public NpcQuestWindow(int npcId): base (npcId)
    {
        InitializeComponent();
    }

    protected override void SetSplashImage(string image)
    {
        splashImage.Source = BitmapTools.ImageFromResource(image);
    }

    protected async void RefreshQuestView()
    {
        await ErrorCatcher.TryAsync(async () =>
        {
            var quests = await ApiCalls.NpcQuestCall.GetOfferedQuestsServerCallAsync(NpcId, GameState.Current.CharacterName);
            questsView.FillData(quests);
        });
    }

    protected void Window_Loaded(object sender, RoutedEventArgs e)
    {
        RefreshQuestView();
    }
}
