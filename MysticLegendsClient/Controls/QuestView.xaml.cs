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
            foreach(var quest in quests)
            {
                questsDict[quest.QuestId] = quest;
                CreateButton(quest);
            }
        }

        private void CreateButton(Quest quest)
        {
            var btn = new QuestButton()
            {
                Title = quest.Name,
                Description = quest.Description,
                // Level = quest.level // TODO: add level
                Acceptance = GetAcceptanceString(quest.AcceptedQuests.FirstOrDefault()),
            };
            btn.Click += ButtonClick;
            btn.Tag = quest.QuestId;
            questPanel.Children.Add(btn);
        }

        private void ButtonClick(object? sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement btn)
            {
                new QuestDetails(questsDict[(int)btn.Tag]).ShowDialog();
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
