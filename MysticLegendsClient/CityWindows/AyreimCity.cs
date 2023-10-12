using MysticLegendsClient.Controls;
using MysticLegendsClient.Resources;
using System.Windows;

namespace MysticLegendsClient.CityWindows
{
    internal class AyreimCity : CityWindow
    {
        public AyreimCity(): base("Ayreim")
        {
            AddButton("Scout", Icons.city_scout);
            AddButton("Dark Alley", Icons.city_darkAlley);
        }
    }
}
