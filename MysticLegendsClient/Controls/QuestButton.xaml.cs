using System.DirectoryServices.ActiveDirectory;
using System.Windows;
using System.Windows.Controls;

namespace MysticLegendsClient.Controls
{
    /// <summary>
    /// Interakční logika pro QuestButton.xaml
    /// </summary>
    public partial class QuestButton : UserControl
    {
        public QuestButton()
        {
            InitializeComponent();
            //DataContext = this;
        }

        public event EventHandler<RoutedEventArgs>? Click;

        private static readonly DependencyProperty title = DependencyProperty.Register("Title", typeof(string), typeof(QuestButton));
        private static readonly DependencyProperty description = DependencyProperty.Register("Description", typeof(string), typeof(QuestButton));
        private static readonly DependencyProperty level = DependencyProperty.Register("Level", typeof(string), typeof(QuestButton));
        private static readonly DependencyProperty acceptance = DependencyProperty.Register("Acceptance", typeof(string), typeof(QuestButton));

        public int QuestId { get; set; }

        public string Title
        {
            get { return (string)GetValue(title); }
            set { SetValue(title, value); }
        }

        public string Description
        {
            get { return (string)GetValue(description); }
            set { SetValue(description, value); }
        }

        public string Level
        {
            get { return (string)GetValue(level); }
            set { SetValue(level, value); }
        }

        public string Acceptance
        {
            get { return (string)GetValue(acceptance); }
            set { SetValue(acceptance, value); }
        }

        private void Button_Click(object? sender, RoutedEventArgs e) => Click?.Invoke(this, e);
    }
}
