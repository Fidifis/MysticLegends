using MysticLegendsShared.Utilities;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;

namespace MysticLegendsClient.Controls
{
    /// <summary>
    /// Interakční logika pro CharacterBanner.xaml
    /// </summary>
    public partial class CharacterBanner : UserControl
    {
        public event EventHandler? ButtonClick;

        public CharacterBanner()
        {
            InitializeComponent();
        }

        private bool detailVisibility = true;
        public bool DetailVisibility
        {
            get => detailVisibility;
            set
            {
                characterClassTxt.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
                characterLevelTxt.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public ImageSource BannerImage
        {
            get => bannerImage.Source;
            set => bannerImage.Source = value;
        }

        public string CharacterName
        {
            get => (string)characterNameTxt.Content;
            set => characterNameTxt.Content = value;
        }

        private CharacterClass characterClass = CharacterClass.Warrior;
        public CharacterClass CharClass
        {
            get => characterClass;
            set
            {
                characterClass = value;
                characterClassTxt.Content = value.ToString();
            }
        }

        private int level = 0;
        public int Level
        {
            get => level;
            set
            {
                level = value;
                characterLevelTxt.VarContent = value.ToString();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ButtonClick?.Invoke(this, new EventArgs());
        }
    }
}
