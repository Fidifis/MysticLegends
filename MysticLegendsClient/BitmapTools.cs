using System.Windows.Media.Imaging;

namespace MysticLegendsClient
{
    internal static class BitmapTools
    {
        public static BitmapImage FromResource(string resourcePath) => new(new Uri($"pack://application:,,,{resourcePath}", UriKind.Absolute));
    }
}
