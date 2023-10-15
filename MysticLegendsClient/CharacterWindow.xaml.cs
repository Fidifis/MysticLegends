using MysticLegendsClasses;
using System.Collections.Immutable;
using System.Windows;

namespace MysticLegendsClient
{
    /// <summary>
    /// Interakční logika pro CharacterWindow.xaml
    /// </summary>
    public partial class CharacterWindow : Window
    {
        private static CharacterWindow? WindowInstance { get; set; } = null;
        public static void ShowWindow(Window? owner)
        {
            WindowInstance ??= new CharacterWindow() { Owner = owner };
            WindowInstance.Show();
            if (WindowInstance.WindowState == WindowState.Minimized) WindowInstance.WindowState = WindowState.Normal;
            WindowInstance.Activate();
        }

        public static void ShowWindow()
        {
            ShowWindow(null);
        }

        public CharacterWindow()
        {
            InitializeComponent();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            WindowInstance = null;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var characterData = new CharacterData
            {
                OwnersAccount = "lol",
                CharacterName = "lol2",
                CharacterClass = CharacterClass.Warrior,
                CurrencyGold = 100,
                Inventory = new(),
                EquipedItems = new List<ItemData>()
                {
                    new()
                    {
                        Name = "Helmet of Ayreim warriors",
                        Icon = "helmet/ayreimWarrior",
                        ItemType = ItemType.Helmet,
                        StackMeansDurability = true,
                        MaxStack = 100,
                        StackCount = 90,
                        BattleStats = new
                        (
                            new BattleStat[] {
                                new() {
                                    BattleStatType = BattleStat.Type.Resilience,
                                    BattleStatMethod = BattleStat.Method.Multiply,
                                    Value = 1.05,
                                },
                            }
                        ),
                    },
                    new()
                    {
                        Name = "Armor of Ayreim warriors",
                        Icon = "bodyArmor/ayreimWarrior",
                        ItemType = ItemType.BodyArmor,
                        StackMeansDurability = true,
                        MaxStack = 100,
                        StackCount = 90,
                        BattleStats = new
                        (
                            new BattleStat[] {
                                new() {
                                    BattleStatType = BattleStat.Type.Resilience,
                                    BattleStatMethod = BattleStat.Method.Add,
                                    Value = 10,
                                },
                                new() {
                                    BattleStatType = BattleStat.Type.Swiftness,
                                    BattleStatMethod = BattleStat.Method.Add,
                                    Value = -1,
                                },
                                new() {
                                    BattleStatType = BattleStat.Type.FireResistance,
                                    BattleStatMethod = BattleStat.Method.Add,
                                    Value = 1.5,
                                },
                            }
                        ),
                    },
                }.ToImmutableList(),
            };
            characterView.FillData(characterData);
        }
    }
}
