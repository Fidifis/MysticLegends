using MysticLegendsShared.Models;
using System.Windows;

namespace MysticLegendsClient.Dialogs
{
    /// <summary>
    /// Interakční logika pro FightResultDialog.xaml
    /// </summary>
    public partial class FightResultDialog : Window
    {
        public readonly struct DisplayData
        {
            public bool Win { get; init; }
            public Mob Enemy { get; init; }
            public IReadOnlyCollection<InventoryItem> DropedItems { get; init; }

            public DisplayData(bool win, Mob enemy, IReadOnlyCollection<InventoryItem> dropedItems)
            {
                Win = win;
                Enemy = enemy;
                DropedItems = dropedItems;
            }
        }

        public FightResultDialog(DisplayData data)
        {
            InitializeComponent();

            var winVis = data.Win ? Visibility.Visible : Visibility.Hidden;
            var loseVis = data.Win ? Visibility.Hidden : Visibility.Visible;

            inventoryView.Items = data.DropedItems;
            winLabel.Visibility = winVis;
            rewardLabel.Visibility = winVis;
            loseLabel.Visibility = loseVis;
        }
    }
}
