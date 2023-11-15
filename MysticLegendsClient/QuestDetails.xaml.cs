using MysticLegendsShared.Models;
using MysticLegendsShared.Utilities;
using System.Windows;

namespace MysticLegendsClient
{
    /// <summary>
    /// Interakční logika pro QuestDetails.xaml
    /// </summary>
    public partial class QuestDetails : Window
    {
        private int questId;
        public QuestDetails(Quest quest)
        {
            InitializeComponent();
            questId = quest.QuestId;
            FillData(quest);
        }

        private void FillData(Quest quest)
        {
            // requirementsView.Items = quest.QuestRequirement // TODO: quest.QuestRequirement by měl být list ne?
            ChangeByQuestState(((QuestState?)quest.AcceptedQuests.FirstOrDefault()?.QuestState) ?? QuestState.NotAccepted);
            title.Text = quest.Name;
            description.Text = quest.Description;
        }

        private void ChangeByQuestState(QuestState state)
        {
            switch (state)
            {
                case QuestState.NotAccepted:
                    acceptButton.Visibility = Visibility.Visible;
                    abandonButton.Visibility = Visibility.Hidden;
                    break;
                case QuestState.Accepted:
                    acceptButton.Visibility = Visibility.Hidden;
                    abandonButton.Visibility = Visibility.Visible;
                    break;
                default:
                    acceptButton.Visibility = Visibility.Hidden;
                    abandonButton.Visibility = Visibility.Hidden;
                    break;
            }
        }

        private void acceptButton_Click(object sender, RoutedEventArgs e)
        {
            _=ErrorCatcher.TryAsync(async () =>
            {
                await ApiCalls.NpcQuestCall.AcceptQuestServerCallAsync(GameState.Current.CharacterName, questId);
                ChangeByQuestState(QuestState.Accepted);
            });
        }

        private void abandonButton_Click(object sender, RoutedEventArgs e)
        {
            _ = ErrorCatcher.TryAsync(async () =>
            {
                await ApiCalls.NpcQuestCall.AbandonQuestServerCallAsync(GameState.Current.CharacterName, questId);
                ChangeByQuestState(QuestState.NotAccepted);
            });
        }
    }
}
