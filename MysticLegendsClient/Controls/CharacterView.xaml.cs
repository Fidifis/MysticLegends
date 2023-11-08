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
        private readonly struct SlotTuple
        {
            public ItemSlot ItemSlot { get; init; }
            public FrameworkElement Root { get; init; }
            public Image Image { get; init; }
        }

        public CharacterView()
        {
            InitializeComponent();

            Slots = new()
            {
                new() { ItemSlot = new(this, (int)ItemType.BodyArmor), Root = bodyArmorSlot, Image = bodyArmorImage },
                new() { ItemSlot = new(this, (int)ItemType.Helmet), Root = helmetSlot, Image = helmetImage },
                new() { ItemSlot = new(this, (int)ItemType.Gloves), Root = glovesSlot, Image = glovesImage },
                new() { ItemSlot = new(this, (int)ItemType.Boots), Root = bootsSlot, Image = bootsImage },
                new() { ItemSlot = new(this, (int)ItemType.Weapon), Root = weaponSlot, Image = weaponImage },
            };

            //Slots.ForEach(slot => slot.Item2.Tag = slot.Item1);
        }

        private readonly List<SlotTuple> Slots;

        //private List<InventoryItem> data = new List<InventoryItem>();
        public override IReadOnlyCollection<InventoryItem> Items
        {
            get => Slots.Where(slot => slot.ItemSlot.Item is not null).Select(slot => slot.ItemSlot.Item!).ToList();
            set => FillData(value);
        }

        //public override ItemSlot GetSlotByPosition(int position) => GetSlotByPosition(position);

        //public override void Update() => FillData(data);

        public override void AddItem(InventoryItem item)
        {
            var iconResource = ItemIcons.ResourceManager.GetString(item.Item.Icon);
            if (iconResource is null)
            {
                // TODO: use Logger
                Console.WriteLine("Icon not found");
                return;
            }
            var bitmap = BitmapTools.FromResource(iconResource);

            var slot = GetSlotByItemType(item.Item.ItemType);

            slot.Image.Source = bitmap;
            slot.ItemSlot.Item = item;
            slot.Root.ToolTip = ItemToolTip.Create(item);
        }

        public override void UpdateItem(InventoryItem updatedItem)
        {
            // idk what to do here. I don't expect the items to change
            throw new NotImplementedException();
        }

        private void FillData(IReadOnlyCollection<InventoryItem> items)
        {
            var battleStats = ComputeBattleStats(items);
            FillBattleStats(battleStats);
            FillEquipedItems(items);
        }

        public void FillData(string characterName, IReadOnlyCollection<InventoryItem> items)
        {
            this.characterName.Content = characterName;

            FillData(items);
        }

        private void ClearEquipedItems()
        {
            foreach (var slot in Slots)
            {
                slot.ItemSlot.Item = null;
                slot.Image.Source = null;
            }
        }

        private void FillEquipedItems(IEnumerable<InventoryItem> equipedItems)
        {
            ClearEquipedItems();
            foreach (var item in equipedItems)
            {
                AddItem(item);
            }
        }

        private SlotTuple GetSlotByItemType(int itemType) =>
            Slots.First(slot => slot.ItemSlot.GridPosition == itemType);

        private SlotTuple GetSlotByRoot(FrameworkElement grid) => Slots.FirstOrDefault(slot => slot.Root == grid);

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
            var slot = GetSlotByRoot((FrameworkElement)sender);
            if (slot.Image.Source is not null)
            {
                HandleDrag(slot.ItemSlot);
            }
        }

        private void Grid_Drop(object sender, DragEventArgs e)
        {
            var target = GetSlotByRoot((FrameworkElement)sender);
            HandleDrop(target.ItemSlot, e);
        }
    }
}