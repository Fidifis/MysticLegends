//namespace MysticLegendsClient
//{
//    internal static class CityModules
//    {
//        public enum CityModule
//        {
//            Blacksmith,
//            Potions,
//            TradeMarket,
//            Guards,
//            Scout,
//            Storage,
//            Major, // must be custom - progression system
//            Bandits, // must be custom - progression system
//        }

//        public static readonly Dictionary<CityModule, Tuple<string, string>> NameIconMap = new()
//        {
//            [CityModule.Blacksmith] = new ("Blacksmith", "/icons/anvil.svg"),
//            [CityModule.Potions] = new ("Potions", "/icons/flask-solid.svg"),
//            [CityModule.TradeMarket] = new ("Trade Market", "/icons/scale-balanced-solid.svg"),
//        };

//        public static readonly List<CityModule> CommonModules = new()
//        {
//            // Scout is not in every city

//            CityModule.Blacksmith,
//            CityModule.Potions,
//            CityModule.TradeMarket,
//            //CityModule.Guards,
//            //CityModule.Storage,
//        };
//    }
//}
