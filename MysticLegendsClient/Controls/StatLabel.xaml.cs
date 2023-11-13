using System.Windows;
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
            //DataContext = this;
        }

        //private string staticContent = "";
        //private string varContent = "";

        private static readonly DependencyProperty staticContent = DependencyProperty.Register("StaticContent", typeof(string), typeof(StatLabel), new PropertyMetadata(UpdateLabel));
        private static readonly DependencyProperty varContent = DependencyProperty.Register("VarContent", typeof(string), typeof(StatLabel), new PropertyMetadata(UpdateLabel));

        private static void UpdateLabel(DependencyObject sender, DependencyPropertyChangedEventArgs e) =>
            ((StatLabel)sender).label.Content = (((StatLabel)sender).GetValue(staticContent) as string ?? "")
            + (((StatLabel)sender).GetValue(varContent) as string ?? "");

        public string StaticContent
        {
            get { return (string)GetValue(staticContent); }
            set
            {
                SetValue(staticContent, value);
            }
        }

        public string VarContent
        {
            get { return (string)GetValue(varContent); }
            set
            {
                SetValue(varContent, value);
            }
        }
    }
}
