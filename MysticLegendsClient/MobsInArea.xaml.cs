using MysticLegendsShared.Models;
using System.Windows;

namespace MysticLegendsClient
{
    /// <summary>
    /// Interakční logika pro MobsInArea.xaml
    /// </summary>
    public partial class MobsInArea : Window
    {
        public MobsInArea(Mob[] mobs)
        {
            InitializeComponent();

            foreach (var mob in mobs)
                mobList.Items.Add(mob.MobName);
        }
    }
}
