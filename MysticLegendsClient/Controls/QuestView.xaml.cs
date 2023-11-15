using MysticLegendsShared.Models;
using MysticLegendsShared.Utilities;
using System.Windows;
using System.Windows.Controls;

namespace MysticLegendsClient.Controls
{
    /// <summary>
    /// Interakční logika pro QuestView.xaml
    /// </summary>
    public partial class QuestView : UserControl
    {
        private readonly Dictionary<int, Quest> questsDict = new();

        public QuestView()
        {
            InitializeComponent();
        }

        public void FillData(IEnumerable<Quest> quests)
        {
            questPanel.Children.Clear();
            foreach (var quest in quests)
            {
                questsDict[quest.QuestId] = quest;
                CreateButton(quest);
            }
        }

        private void CreateButton(Quest quest)
        {
            var btn = new QuestButton()
            {
                QuestId = quest.QuestId,
                Title = quest.Name,
                Description = quest.Description,
                // Level = quest.level // TODO: add level
                Acceptance = GetAcceptanceString(quest.AcceptedQuests.FirstOrDefault()),
                Height = 100,
            };
            btn.Click += ButtonClick;
            questPanel.Children.Add(btn);
        }

        private void ButtonClick(object? sender, RoutedEventArgs e)
        {
            if (sender is QuestButton btn)
            {
                new QuestDetails(questsDict[btn.QuestId]).ShowDialog();
            }
        }

        private static string GetAcceptanceString(AcceptedQuest? quest) => (QuestState)(quest?.QuestState ?? 0) switch
        {
            QuestState.NotAccepted => "Not accepted",
            QuestState.Accepted => "Accepted",
            QuestState.Completed => "Completed",
            _ => "Undefined"
        };
    }
}
