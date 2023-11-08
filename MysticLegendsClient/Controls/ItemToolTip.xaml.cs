using MysticLegendsShared.Models;
using MysticLegendsShared.Utilities;
using System.Windows.Controls;

namespace MysticLegendsClient.Controls
{
    /// <summary>
    /// Interakční logika pro ItemToolTip.xaml
    /// </summary>
    public partial class ItemToolTip : UserControl
    {
        public ItemToolTip()
        {
            InitializeComponent();
        }

        public string? TitleLabel
        {
            get => titleLabel.Content.ToString();
            set => titleLabel.Content = value;
        }
        public string? StatLabel
        {
            get => statLabel.Content.ToString();
            set => statLabel.Content = value;
        }
        public string? TagLabel
        {
            get => tagLabel.Content.ToString();
            set => tagLabel.Content = value;
        }

        public static ItemToolTip? Create(InventoryItem? item)
        {
            if (item is null) return null;
            var statsString = new BattleStats(item.BattleStats).ToString();
            var levelString = item.Level is null ? "" : $"Level: {item.Level}";
            var priceString = item.Price is null ? "" : $"Price: {item.Price.PriceGold}";

            return new ItemToolTip { TitleLabel = item.Item.Name, StatLabel = $"{levelString}\n{statsString}", TagLabel = priceString };
        }
    }
}
