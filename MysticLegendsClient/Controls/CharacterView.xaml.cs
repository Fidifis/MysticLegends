using MysticLegendsShared.Utilities;
using MysticLegendsClient.Resources;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MysticLegendsClient.Controls
{
    /// <summary>
    /// Interakční logika pro CharacterView.xaml
    /// </summary>
    public partial class CharacterView : UserControl, IItemView
    {
        public event IItemView.ItemDropEventHandler? ItemDropEvent;

        public ICollection<ItemViewRelation> ViewRelations => throw new NotImplementedException();

        public CharacterView()
        {
            InitializeComponent();

            bodyArmorSlot.Tag = new ItemSlot(this, (int)ItemType.BodyArmor);
            helmetSlot.Tag = new ItemSlot(this, (int)ItemType.Helmet);
            glovesSlot.Tag = new ItemSlot(this, (int)ItemType.Gloves);
            bootsSlot.Tag = new ItemSlot(this, (int)ItemType.Boots);
            weaponSlot.Tag = new ItemSlot(this, (int)ItemType.Weapon);
        }

        public void PutItems(ICollection<IViewableItem> items)
        {
            PutItems(items.Cast<InventoryItemViewable>());
        }

        public void FillData(string characterName, IEnumerable<InventoryItemViewable> items)
        {
            this.characterName.Content = characterName;

            PutItems(items);
        }

        public void PutItems(IEnumerable<InventoryItemViewable> items)
        {
            var battleStats = ComputeBattleStats(items);
            FillBattleStats(battleStats);
            FillEquipedItems(items);
        }

        private void ClearEquipedItems()
        {
            var images = new Image[] { weaponImage, bodyArmorImage, helmetImage, glovesImage, bootsImage };
            foreach (var image in images)
            {
                ((ItemSlot)image.Tag).Item = null;
                image.Source = null;
            }
        }

        private void FillEquipedItems(IEnumerable<InventoryItemViewable> equipedItems)
        {
            ClearEquipedItems();
            foreach (var item in equipedItems)
            {
                var iconResource = ItemIcons.ResourceManager.GetString(item.Source.Item.Icon);
                if (iconResource is null)
                {
                    // TODO: use Logger
                    Console.WriteLine("Icon not found");
                    continue;
                }
                var bitmap = BitmapTools.FromResource(iconResource);

                Image? imageControl = GetImageByItemType((ItemType)item.Source.Item.ItemType);

                if (imageControl is null) continue;
                imageControl.Source = bitmap;
                ((ItemSlot)imageControl.Tag).Item = item;
            }
        }

        private Image? GetImageByItemType(ItemType itemType) => itemType switch
        {
            ItemType.Weapon => weaponImage,
            ItemType.BodyArmor => bodyArmorImage,
            ItemType.Helmet => helmetImage,
            ItemType.Gloves => glovesImage,
            ItemType.Boots => bootsImage,
            _ => null,
        };

        private BattleStats ComputeBattleStats(IEnumerable<InventoryItemViewable> items)
        {
            var battleStats = from item in items where item.Source.BattleStats is not null select new BattleStats(item.Source.BattleStats);

            return new BattleStats(battleStats);
        }

        public void FillBattleStats(BattleStats battleStats)
        {
            strength.VarContent = battleStats.Get(CBattleStat.Type.Strength).Value.ToString();
            dexterity.VarContent = battleStats.Get(CBattleStat.Type.Dexterity).Value.ToString();
            intelligence.VarContent = battleStats.Get(CBattleStat.Type.Intelligence).Value.ToString();

            physicalDamage.VarContent = battleStats.Get(CBattleStat.Type.PhysicalDamage).Value.ToString();
            swiftness.VarContent = battleStats.Get(CBattleStat.Type.Swiftness).Value.ToString();
            magicStrength.VarContent = battleStats.Get(CBattleStat.Type.MagicStrength).Value.ToString();

            resilience.VarContent = battleStats.Get(CBattleStat.Type.Resilience).Value.ToString();
            evade.VarContent = battleStats.Get(CBattleStat.Type.Evade).Value.ToString();
            magicProtection.VarContent = battleStats.Get(CBattleStat.Type.MagicProtection).Value.ToString();

            fireResistance.VarContent = battleStats.Get(CBattleStat.Type.FireResistance).Value.ToString();
            poisonResistance.VarContent = battleStats.Get(CBattleStat.Type.PoisonResistance).Value.ToString();
            arcaneResistance.VarContent = battleStats.Get(CBattleStat.Type.ArcaneResistance).Value.ToString();
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (((FrameworkElement)sender).Tag is ItemSlot slot && GetImageByItemType((ItemType)slot.GridPosition)?.Source is not null)
            {
                var data = new DataObject(typeof(ItemSlot), slot);
                DragDrop.DoDragDrop((DependencyObject)sender, data, DragDropEffects.Move);
            }
        }

        private void Grid_Drop(object sender, DragEventArgs e)
        {
            var target = (FrameworkElement)sender;

            if (e.Data.GetDataPresent(typeof(FrameworkElement)))
            {
                var sourceSlot = (ItemSlot)e.Data.GetData(typeof(ItemSlot));
                var targetSlot = (ItemSlot)target.Tag;

                ItemDropEvent?.Invoke(sourceSlot.Owner, new ItemDropEventArgs(sourceSlot, targetSlot));
            }
        }
    }
}