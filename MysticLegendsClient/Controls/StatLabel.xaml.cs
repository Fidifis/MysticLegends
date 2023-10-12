using System.Windows.Controls;

namespace MysticLegendsClient.Controls
{
    /// <summary>
    /// Interakční logika pro StatLabel.xaml
    /// </summary>
    public partial class StatLabel : UserControl
    {
        public StatLabel()
        {
            InitializeComponent();
        }

        private string staticContent = "";
        private string varContent = "";

        private void UpdateLabel() => label.Content = staticContent + varContent;

        public string StaticContent
        {
            get { return staticContent; }
            set
            {
                staticContent = value;
                UpdateLabel();
            }
        }

        public string VarContent
        {
            get { return varContent; }
            set
            {
                varContent = value;
                UpdateLabel();
            }
        }
    }
}
