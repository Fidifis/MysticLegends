using MysticLegendsClasses;
using MysticLegendsClient.Resources;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MysticLegendsClient.Controls
{
    /// <summary>
    /// Interakční logika pro CharacterView.xaml
    /// </summary>
    public partial class CharacterView : UserControl
    {
        public delegate void ItemDrop(InventoryItemContext source, InventoryItemContext target);
        public ItemDrop? ItemDropCallback { get; set; }

        public CharacterView()
        {
            InitializeComponent();

            bodyArmorSlot.Tag = new InventoryItemContext(this, (int)ItemType.BodyArmor);
            helmetSlot.Tag = new InventoryItemContext(this, (int)ItemType.Helmet);
            glovesSlot.Tag = new InventoryItemContext(this, (int)ItemType.Gloves);
            bootsSlot.Tag = new InventoryItemContext(this, (int)ItemType.Boots);
            weaponSlot.Tag = new InventoryItemContext(this, (int)ItemType.Weapon);
        }

        public void FillData(CharacterData characterData)
        {
            characterName.Content = characterData.CharacterName;
            if (characterData.EquipedItems is not null)
            {
                var battleStats = ComputeBattleStats(characterData.EquipedItems!);
                FillBattleStats(battleStats);
                FillEquipedItems(characterData.EquipedItems!);
                characterName.Content = characterData.CharacterName;
            }
        }

        private void ClearEquipedItems()
        {
            var images = new Image[] { weaponImage, bodyArmorImage, helmetImage, glovesImage, bootsImage };
            foreach (var image in images)
                image.Source = null;
        }

        private void FillEquipedItems(IEnumerable<ItemData> equipedItems)
        {
            ClearEquipedItems();
            foreach (var item in equipedItems)
            {
                var iconResource = Items.ResourceManager.GetString(item.Icon);
                if (iconResource is null)
                {
                    // TODO: use Logger
                    Console.WriteLine("Icon not found");
                    continue;
                }
                var bitmap = BitmapTools.FromResource(iconResource);

                Image? imageControl = GetImageByItemType(item.ItemType);

                if (imageControl is null) continue;
                imageControl.Source = bitmap;
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

        private BattleStats ComputeBattleStats(IEnumerable<ItemData> items)
        {
            var battleStats = from item in items where item.BattleStats is not null select item.BattleStats!.Value;

            return new BattleStats(battleStats);
        }

        public void FillBattleStats(BattleStats battleStats)
        {
            var stats = battleStats.Stats;

            strength.VarContent = stats.Get(BattleStat.Type.Strength).Value.ToString();
            dexterity.VarContent = stats.Get(BattleStat.Type.Dexterity).Value.ToString();
            intelligence.VarContent = stats.Get(BattleStat.Type.Intelligence).Value.ToString();

            physicalDamage.VarContent = stats.Get(BattleStat.Type.PhysicalDamage).Value.ToString();
            swiftness.VarContent = stats.Get(BattleStat.Type.Swiftness).Value.ToString();
            magicStrength.VarContent = stats.Get(BattleStat.Type.MagicStrength).Value.ToString();

            resilience.VarContent = stats.Get(BattleStat.Type.Resilience).Value.ToString();
            evade.VarContent = stats.Get(BattleStat.Type.Evade).Value.ToString();
            magicProtection.VarContent = stats.Get(BattleStat.Type.MagicProtection).Value.ToString();

            fireResistance.VarContent = stats.Get(BattleStat.Type.FireResistance).Value.ToString();
            poisonResistance.VarContent = stats.Get(BattleStat.Type.PoisonResistance).Value.ToString();
            arcaneResistance.VarContent = stats.Get(BattleStat.Type.ArcaneResistance).Value.ToString();
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (((FrameworkElement)sender).Tag is InventoryItemContext context && GetImageByItemType((ItemType)context.Id)?.Source is not null)
            {
                var data = new DataObject(typeof(FrameworkElement), sender);
                DragDrop.DoDragDrop((DependencyObject)sender, data, DragDropEffects.Move);
            }
        }

        private void Grid_Drop(object sender, DragEventArgs e)
        {
            var target = (FrameworkElement)sender;

            if (e.Data.GetDataPresent(typeof(FrameworkElement)))
            {
                var source = (FrameworkElement)e.Data.GetData(typeof(FrameworkElement));
                ItemDropCallback?.Invoke((InventoryItemContext)source.Tag, (InventoryItemContext)target.Tag);
            }
        }
    }
}