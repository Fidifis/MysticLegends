using System.Windows.Media.Imaging;

namespace MysticLegendsClient
{
    internal static class BitmapTools
    {
        public static BitmapImage ImageFromResource(string resourcePath) => new(new Uri($"pack://application:,,,{resourcePath}", UriKind.Absolute));
    }
}
