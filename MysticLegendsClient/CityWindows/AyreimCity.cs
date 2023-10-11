using MysticLegendsClient.Controls;
using System.Windows;

namespace MysticLegendsClient.CityWindows
{
    internal class AyreimCity : CityWindow
    {
        public AyreimCity()
        {
            cityNameLabel.Content = "Ayreim";
            CityModulesPanel.Children.Add(new CityModuleButton { InnerPadding="20 10 20 10", Margin=new Thickness(0, 0, 0, 10), TextGap=50, FontSize=20, UniformSvgSize="50", SvgSource="/icons/anvil.svg", LabelText="Dark Alley" });
        }
    }
}
