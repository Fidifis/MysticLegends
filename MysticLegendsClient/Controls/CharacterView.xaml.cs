using MysticLegendsClasses;
using MysticLegendsClient.Resources;
using System.Windows.Controls;

namespace MysticLegendsClient.Controls
{
    /// <summary>
    /// Interakční logika pro CharacterView.xaml
    /// </summary>
    public partial class CharacterView : UserControl
    {
        public CharacterView()
        {
            InitializeComponent();
        }

        public void FillData(CharacterData characterData)
        {
            characterName.Content = characterData.CharacterName;
            if (characterData.EquipedItems is not null)
            {
                var battleStats = ComputeBattleStats(characterData.EquipedItems!);
                FillBattleStats(battleStats);
                FillEquipedItems(characterData.EquipedItems!);
            }
        }

        private void FillEquipedItems(IEnumerable<ItemData> equipedItems)
        {
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

                Image? imageControl = item.ItemType switch
                {
                    ItemType.Helmet => helmetImage,
                    ItemType.BodyArmor => bodyArmorImage,
                    ItemType.Gloves => glovesImage,
                    ItemType.Boots => bootsImage,
                    ItemType.Weapon => weaponImage,
                    _ => null,
                };

                if (imageControl is null) continue;
                imageControl.Source = bitmap;
            }
        }

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
    }
}