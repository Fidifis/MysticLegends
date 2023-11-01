﻿using MysticLegendsShared.Utilities;
using MysticLegendsShared.Models;
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

        public void FillData(Character characterData)
        {
            characterName.Content = characterData.CharacterName;
            if (characterData.InventoryItems is not null)
            {
                var battleStats = ComputeBattleStats(characterData.InventoryItems);
                FillBattleStats(battleStats);
                FillEquipedItems(characterData.InventoryItems!);
                characterName.Content = characterData.CharacterName;
            }
        }

        private void ClearEquipedItems()
        {
            var images = new Image[] { weaponImage, bodyArmorImage, helmetImage, glovesImage, bootsImage };
            foreach (var image in images)
                image.Source = null;
        }

        private void FillEquipedItems(IEnumerable<InventoryItem> equipedItems)
        {
            ClearEquipedItems();
            foreach (var item in equipedItems)
            {
                var iconResource = Items.ResourceManager.GetString(item.Item.Icon);
                if (iconResource is null)
                {
                    // TODO: use Logger
                    Console.WriteLine("Icon not found");
                    continue;
                }
                var bitmap = BitmapTools.FromResource(iconResource);

                Image? imageControl = GetImageByItemType((ItemType)item.Item.ItemType);

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

        private BattleStats ComputeBattleStats(IEnumerable<InventoryItem> items)
        {
            var battleStats = from item in items where item.BattleStats is not null select new BattleStats(item.BattleStats);

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