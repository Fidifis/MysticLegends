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
        private readonly IEnumerable<Mob> mobs;

        public MobsInAreaDialog(IEnumerable<Mob> mobs)
        {
            InitializeComponent();

            this.mobs = mobs;

            var namesSet = new HashSet<string>(mobs.Select(mob => mob.MobName));

            foreach (var mob in namesSet)
                mobList.Items.Add(mob);
        }

        private void MobList_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            RedrawLevels();
            SelectedMob = mobs.Where(mob => mob.MobName == (string?)mobList.SelectedItem && mob.Level == (int?)levelList.SelectedItem).SingleOrDefault();
            RedrawItems();
        }

        private void RedrawLevels()
        {
            levelList.Items.Clear();

            var levels = mobs.Where(mob => mob.MobName == (string)mobList.SelectedItem).Select(mob => mob.Level);
            foreach (var level in levels)
            {
                levelList.Items.Add(level);
            }
            if (levelList.Items.Count > 0)
                levelList.SelectedIndex = 0;
        }

        private void RedrawItems()
        {
            rewardView.Items = mobs.SingleOrDefault(mob => mob == SelectedMob)?.MobItemDrops.Select(drop => drop.Item).Select(item => new InventoryItem
            {
                ItemId = item.ItemId,
                Item = item,
                StackCount = 1,
            })
                ?? Array.Empty<InventoryItem>();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedMob is null)
                return;
            DialogResult = true;
        }
    }
}
