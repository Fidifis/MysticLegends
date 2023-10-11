using System.Windows;
using System.Windows.Controls;

namespace MysticLegendsClient.Controls
{
    /// <summary>
    /// Interakční logika pro CityModuleButton.xaml
    /// </summary>
    public partial class CityModuleButton : UserControl
    {
        public CityModuleButton()
        {
            InitializeComponent();
            DataContext = this;
        }

        public event RoutedEventHandler? Click;

        private void Button_Click(object sender, RoutedEventArgs e) => Click?.Invoke(sender, e);

        public static readonly DependencyProperty svgSource = DependencyProperty.Register("SvgSource", typeof(string), typeof(CityModuleButton));
        public static readonly DependencyProperty labelText = DependencyProperty.Register("LabelText", typeof(string), typeof(CityModuleButton));
        public static readonly DependencyProperty uniformSvgSize = DependencyProperty.Register("UniformSvgSize", typeof(string), typeof(CityModuleButton));
        public static readonly DependencyProperty innerPadding = DependencyProperty.Register("InnerPadding", typeof(string), typeof(CityModuleButton));
        public static readonly DependencyProperty innerMargin = DependencyProperty.Register("InnerMargin", typeof(string), typeof(CityModuleButton));
        public static readonly int textGap = 0;

        public string SvgSource
        {
            get { return (string)GetValue(svgSource); }
            set { SetValue(svgSource, value); }
        }

        public string LabelText
        {
            get { return (string)GetValue(labelText); }
            set { SetValue(labelText, value); }
        }

        public string UniformSvgSize
        {
            get { return (string)GetValue(uniformSvgSize); }
            set { SetValue(uniformSvgSize, value); }
        }

        public string InnerPadding
        {
            get { return (string)GetValue(innerPadding); }
            set { SetValue(innerPadding, value); }
        }

        private string InnerMargin
        {
            get { return (string)GetValue(innerMargin); }
            set { SetValue(innerMargin, value); }
        }

        public int TextGap
        {
            get { return textGap; }
            set { InnerMargin = $"{value} 0 0 0"; }
        }
    }
}
