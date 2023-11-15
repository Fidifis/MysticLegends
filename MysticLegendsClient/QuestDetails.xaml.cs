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
        private readonly int questId;
        public QuestDetails(Quest quest)
        {
            InitializeComponent();
            questId = quest.QuestId;
            FillData(quest);
        }

        private void FillData(Quest quest)
        {
            requirementsView.Items = quest.QuestRequirements.Where(requirement => requirement.Item is not null).Select(requirement => new InventoryItem()
            {
                Item = requirement.Item!,
                ItemId = requirement.Item!.ItemId,
                StackCount = requirement.Amount,
            }).ToList();
            ChangeByQuestState(((QuestState?)quest.AcceptedQuests.FirstOrDefault()?.QuestState) ?? QuestState.NotAccepted);
            title.Text = quest.Name;
            description.Text = quest.Description;
            level.VarContent = quest.Level.ToString();
            reward.VarContent = quest.QuestReward?.CurrencyGold.ToString() ?? "";
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
