using MysticLegendsShared.Utilities;
using MysticLegendsClient.Resources;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MysticLegendsShared.Models;

namespace MysticLegendsClient.Controls
{
    /// <summary>
    /// Interakční logika pro CharacterView.xaml
    /// </summary>
    public partial class CharacterView : ItemViewUserControl
    {
        public CharacterView()
        {
            InitializeComponent();

            Slots = new()
            {
                new(bodyArmorSlot, bodyArmorImage),
                new(helmetSlot, helmetImage),
                new(glovesSlot, glovesImage),
                new(bootsSlot, bootsImage),
                new(weaponSlot, weaponImage),
            };

            bodyArmorSlot.Tag = new ItemSlot(this, (int)ItemType.BodyArmor);
            helmetSlot.Tag = new ItemSlot(this, (int)ItemType.Helmet);
            glovesSlot.Tag = new ItemSlot(this, (int)ItemType.Gloves);
            bootsSlot.Tag = new ItemSlot(this, (int)ItemType.Boots);
            weaponSlot.Tag = new ItemSlot(this, (int)ItemType.Weapon);
        }

        private List<Tuple<Grid, Image>> Slots;

        //Rework
        private ICollection<InventoryItem> data = new List<InventoryItem>();
        public override ICollection<InventoryItem> Items
        {
            get => data;
            set
            {
                data = value;
                FillData(data);
            }
        }

        //public override ItemSlot GetSlotByPosition(int position) => GetSlotByPosition(position);

        public override void Update() => FillData(data);

        private void FillData(ICollection<InventoryItem> items)
        {
            var battleStats = ComputeBattleStats(items);
            FillBattleStats(battleStats);
            FillEquipedItems(items);
        }

        public void FillData(string characterName, ICollection<InventoryItem> items)
        {
            this.characterName.Content = characterName;

            FillData(items);
        }

        private void ClearEquipedItems()
        {
            var images = new Image[] { weaponImage, bodyArmorImage, helmetImage, glovesImage, bootsImage };
            foreach (var slot in Slots)
            {
                ((ItemSlot)slot.Item1.Tag).Item = null;
                slot.Item2.Source = null;
            }
        }

        private void FillEquipedItems(IEnumerable<InventoryItem> equipedItems)
        {
            ClearEquipedItems();
            foreach (var item in equipedItems)
            {
                var iconResource = ItemIcons.ResourceManager.GetString(item.Item.Icon);
                if (iconResource is null)
                {
                    // TODO: use Logger
                    Console.WriteLine("Icon not found");
                    continue;
                }
                var bitmap = BitmapTools.FromResource(iconResource);

                var imageControl = GetImageByItemType((ItemType)item.Item.ItemType);

                imageControl.Item2.Source = bitmap;
                ((ItemSlot)imageControl.Item1.Tag).Item = item;
            }
        }

        private Tuple<Grid, Image> GetImageByItemType(ItemType itemType) => itemType switch
        {
            ItemType.Weapon => new(weaponSlot, weaponImage),
            ItemType.BodyArmor => new(bodyArmorSlot, bodyArmorImage),
            ItemType.Helmet => new(helmetSlot, helmetImage),
            ItemType.Gloves => new(glovesSlot, glovesImage),
            ItemType.Boots => new(bootsSlot, bootsImage),
            _ => throw new Exception("Wrong item type"),
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
            if (((FrameworkElement)sender).Tag is ItemSlot slot && GetImageByItemType((ItemType)slot.GridPosition).Item2.Source is not null)
            {
                HandleDrag(slot);
            }
        }

        private void Grid_Drop(object sender, DragEventArgs e)
        {
            var target = (FrameworkElement)sender;
            var targetSlot = (ItemSlot)target.Tag;
            HandleDrop(targetSlot, e);
        }
    }
}