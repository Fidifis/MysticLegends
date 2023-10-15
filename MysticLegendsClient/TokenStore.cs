using System.IO;
using System.Text;
using System.Text.Json;

namespace MysticLegendsClient
{
    internal static class TokenStore
    {
        public static string? AccessToken { get; set; }

        public static readonly string StorePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Mystic Legends\tokens.json";
        private static readonly Encoding encoding = Encoding.UTF8;
        private const string refreshTokenString = "refreshToken";

        public static string? GetRefreshToken()
        {
            var data = ReadJsonFile(StorePath);
            if (data?.ContainsKey(refreshTokenString) == true)
            {
                var tokenBytes = Convert.FromBase64String(data[refreshTokenString]);
                return encoding.GetString(tokenBytes);
            }
            return null;
        }

        public static void SaveRefreshToken(string token)
        {
            var dir = Path.GetDirectoryName(StorePath);

            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir!);

            var data = ReadJsonFile(StorePath) ?? new();

            var tokenBytes = encoding.GetBytes(token);
            var base64Token = Convert.ToBase64String(tokenBytes);

            data[refreshTokenString] = base64Token;

            var json = JsonSerializer.Serialize(data);
            File.WriteAllText(StorePath, json);
        }

        private static Dictionary<string,string>? ReadJsonFile(string path)
        {
            if (!File.Exists(path))
                return null;

            var stream = File.OpenRead(path);
            return JsonSerializer.Deserialize<Dictionary<string, string>>(stream);
        }
    }
}
