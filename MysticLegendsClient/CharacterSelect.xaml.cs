using System.Windows;
using MysticLegendsClient.Controls;
using MysticLegendsClient.Dialogs;
using MysticLegendsShared.Utilities;

namespace MysticLegendsClient
{
    /// <summary>
    /// Interakční logika pro CharacterSelect.xaml
    /// </summary>
    public partial class CharacterSelect : Window
    {
        public struct CharacterDisplayData
        {
            public string Name { get; set; }
            public int Level { get; set; }
            public CharacterClass CharacterClass { get; set; }

            public CharacterDisplayData(string name, int level, CharacterClass characterClass)
            {
                Name = name;
                Level = level;
                CharacterClass = characterClass;
            }
        }

        public string? ResultCharacterName { get; private set; }
        private bool createMode = false;
        private string userWhenCreating;
        private IEnumerable<CharacterDisplayData> charactersToFill;

        public CharacterSelect(string userWhenCreating, IEnumerable<CharacterDisplayData> characters)
        {
            InitializeComponent();
            this.userWhenCreating = userWhenCreating;
            charactersToFill = characters;
            ButtonsVisibility(false);
            FillView(characters, true);
        }

        private void FillView(IEnumerable<CharacterDisplayData> characters, bool detailed)
        {
            slotsPanel.Children.Clear();
            foreach (var character in characters)
            {
                var characterBanner = new CharacterBanner()
                {
                    BannerImage = BitmapTools.ImageFromResource(GetImageForClass(character.CharacterClass)),
                    CharacterName = character.Name,
                    CharClass = character.CharacterClass,
                    Level = character.Level,
                    DetailVisibility = detailed,
                    Margin = new Thickness(10),
                };
                characterBanner.ButtonClick += SelectClick;
                slotsPanel.Children.Add(characterBanner);
            }
        }

        private async void SelectClick(object? sender, EventArgs e)
        {
            if (sender is CharacterBanner banner)
            {
                if (createMode)
                {
                    var character = await CreateCharacterOfClass(userWhenCreating, banner.CharClass);
                    if (character is not null)
                    {
                        ResultCharacterName = character;
                        DialogResult = true;
                        Close();
                    }
                }
                else
                {
                    ResultCharacterName = banner.CharacterName;
                    DialogResult = true;
                    Close();
                }
            }
        }

        private async Task<string?> CreateCharacterOfClass(string username, CharacterClass characterClass)
        {
            var enterNameDial = new EnterTextDialog("Give your character a name", "Enter a name");
            if (enterNameDial.ShowDialog() != true)
                return null;

            var name = enterNameDial.EnteredText.Trim();
            var retunedName = await ErrorCatcher.TryAsync(async () =>
            {
                return await ApiCalls.UserCall.CreateCharacter(username, name, characterClass);
            });
            return name == retunedName ? name : null;
        }

        private void Create_Click(object sender, RoutedEventArgs e)
        {
            createMode = true;
            ButtonsVisibility(true);

            FillView(Enum.GetValues<CharacterClass>().Select(clas =>
                new CharacterDisplayData()
                {
                    Name = clas.ToString(),
                    CharacterClass = clas,
                }).ToList(),
                false);
        }

        private string GetImageForClass(CharacterClass characterClass)
        {
            return characterClass switch
            {
                CharacterClass.Warrior => "/images/Characters/Warrior.png",
                CharacterClass.Mage => "/images/Characters/Mage.png",
                CharacterClass.Assassin => "/images/Characters/Assassin.png",
                _=>throw new NotImplementedException("unknown class")
            };
        }

        private void ButtonsVisibility(bool showCreate)
        {
            heading.Content = showCreate ? "Choose class" : "Choose your character";
            createBtn.Visibility = showCreate ? Visibility.Hidden : Visibility.Visible;
            backBtn.Visibility = showCreate ? Visibility.Visible : Visibility.Hidden;
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            createMode = false;
            ButtonsVisibility(false);

            FillView(charactersToFill, true);
        }
    }
}
