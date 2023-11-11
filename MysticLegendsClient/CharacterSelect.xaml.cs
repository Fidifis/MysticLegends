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
            public string Image { get; set; }

            public CharacterDisplayData(string name, int level, CharacterClass characterClass, string image)
            {
                Name = name;
                Level = level;
                CharacterClass = characterClass;
                Image = image;
            }
        }

        public string? ResultCharacterName { get; private set; }
        private bool createMode = false;
        private string userWhenCreating;
        private IReadOnlyCollection<CharacterDisplayData> charactersToFill;

        public CharacterSelect(string userWhenCreating, IReadOnlyCollection<CharacterDisplayData> characters)
        {
            InitializeComponent();
            this.userWhenCreating = userWhenCreating;
            charactersToFill = characters;
            FillView(characters, true);
        }

        private void FillView(IReadOnlyCollection<CharacterDisplayData> characters, bool detailed)
        {
            slotsPanel.Children.Clear();
            foreach (var character in characters)
            {
                var characterBanner = new CharacterBanner()
                {
                    BannerImage = BitmapTools.ImageFromResource(character.Image),
                    CharacterName = character.Name,
                    CharClass = character.CharacterClass,
                    Level = character.Level,
                    DetailVisibility = detailed,
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
                        Close();
                    }    
                }
                else
                    ResultCharacterName = banner.CharacterName;
            }
        }

        private async Task<string?> CreateCharacterOfClass(string username, CharacterClass characterClass)
        {
            var enterNameDial = new EnterTextDialog("Give your character a name", "Enter a name");
            if (enterNameDial.ShowDialog() != true)
                return null;

            var name = enterNameDial.EnteredText.Trim();
            await ApiCalls.UserCall.CreateCharacter(username, name, characterClass);
            return name;
        }

        private void Create_Click(object sender, RoutedEventArgs e)
        {
            createMode = true;
            ButtonsVisibility(false);

            FillView(Enum.GetValues<CharacterClass>().Select(clas =>
                new CharacterDisplayData()
                {
                    Name = clas.ToString(),
                    CharacterClass = clas,
                    Image = GetImageForClass(clas)
                }).ToList(),
                false);
        }

        private string GetImageForClass(CharacterClass characterClass)
        {
            return characterClass switch
            {
                CharacterClass.Warrior => "/images/Characters/Mage.png",
                CharacterClass.Mage => "/images/Characters/Mage.png",
                CharacterClass.Assasin => "/images/Characters/Mage.png",
                _=>throw new NotImplementedException("unknown class")
            };
        }

        private void ButtonsVisibility(bool showCreate)
        {
            createBtn.Visibility = showCreate ? Visibility.Visible : Visibility.Hidden;
            backBtn.Visibility = showCreate ? Visibility.Hidden : Visibility.Visible;
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            createMode = false;
            ButtonsVisibility(true);

            FillView(charactersToFill, true);
        }
    }
}
