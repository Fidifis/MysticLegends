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

            inventoryView.Items = data.DropedItems;
            winLabel.Content = data.Win ? "Win" : "Loose";
        }
    }
}
