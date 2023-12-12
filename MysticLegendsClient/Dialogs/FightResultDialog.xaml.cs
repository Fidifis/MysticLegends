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
            public Mob Enemy { get; init; }
            public InventoryItem[] DropedItems { get; init; }

            public DisplayData(Mob enemy, InventoryItem[] dropedItems)
            {
                Enemy = enemy;
                DropedItems = dropedItems;
            }
        }

        public FightResultDialog(DisplayData data)
        {
            InitializeComponent();
        }
    }
}
