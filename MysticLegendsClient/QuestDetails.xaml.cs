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
        public event EventHandler<UpdateEventArgs<QuestState>>? QuestStateUpdatedEvent;

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
            title.Text = quest.Name;
            description.Text = quest.Description;
            level.VarContent = quest.Level.ToString();
            reward.VarContent = quest.QuestReward?.CurrencyGold.ToString() ?? "";
            xps.VarContent = quest.QuestReward?.Xp.ToString() ?? "";

            ChangeAllButtonsState(quest);
        }

        private void ChangeAllButtonsState(Quest quest)
        {
            var questState = GetQuestState(quest);
            ChangeAllButtonsState(questState);
        }

        private void ChangeAllButtonsState(QuestState questState)
        {
            ChangeByQuestState(questState);
            CompletableButton(questState);
        }

        private QuestState GetQuestState(Quest quest) => ((QuestState?)quest.AcceptedQuests.FirstOrDefault()?.QuestState) ?? QuestState.NotAccepted;
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

        private async void CompletableButton(QuestState questState)
        {
            if (questState != QuestState.Accepted)
            {
                completeButton.Visibility = Visibility.Hidden;
                return;
            }

            await ErrorCatcher.TryAsync(async () =>
            {
                var vis = await ApiCalls.NpcQuestCall.GetQuestCompletableCallAsync(questId);
                completeButton.Visibility = vis ? Visibility.Visible : Visibility.Hidden;
            });
        }

        private void acceptButton_Click(object sender, RoutedEventArgs e)
        {
            _=ErrorCatcher.TryAsync(async () =>
            {
                await ApiCalls.NpcQuestCall.AcceptQuestServerCallAsync(GameState.Current.CharacterName, questId);
                ChangeAllButtonsState(QuestState.Accepted);
                QuestStateUpdatedEvent?.Invoke(this, new(QuestState.Accepted));
            });
        }

        private void abandonButton_Click(object sender, RoutedEventArgs e)
        {
            _ = ErrorCatcher.TryAsync(async () =>
            {
                await ApiCalls.NpcQuestCall.AbandonQuestServerCallAsync(GameState.Current.CharacterName, questId);
                ChangeAllButtonsState(QuestState.NotAccepted);
                QuestStateUpdatedEvent?.Invoke(this, new(QuestState.NotAccepted));
            });
        }
    }
}
