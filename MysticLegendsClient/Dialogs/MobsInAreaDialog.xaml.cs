using MysticLegendsShared.Models;
using System.Windows;

namespace MysticLegendsClient.Dialogs
{
    /// <summary>
    /// Interakční logika pro MobsInAreaDialog.xaml
    /// </summary>
    public partial class MobsInAreaDialog : Window
    {
        public Mob? SelectedMob { get; private set; }
        private Mob[] mobs;

        public MobsInAreaDialog(Mob[] mobs)
        {
            InitializeComponent();

            this.mobs = mobs;

            var namesSet = new HashSet<string>(mobs.Select(mob => mob.MobName));

            foreach (var mob in namesSet)
                mobList.Items.Add(mob);
        }

        private void mobList_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            levelList.Items.Clear();

            var levels = mobs.Where(mob => mob.MobName == (string)mobList.SelectedItem).Select(mob => mob.Level);
            foreach (var level in levels)
            {
                levelList.Items.Add(level);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SelectedMob = mobs.Where(mob => mob.MobName == (string?)mobList.SelectedItem && mob.Level == (int?)levelList.SelectedItem).SingleOrDefault();
            if (SelectedMob is null)
                return;
            DialogResult = true;
        }
    }
}
