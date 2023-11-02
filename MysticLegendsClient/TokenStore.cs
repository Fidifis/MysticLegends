using System.IO;
using System.Text;
using System.Text.Json;

namespace MysticLegendsClient
{
    internal class TokenStore
    {
        public string? AccessToken { get; set; }

        public readonly string StorePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Mystic Legends\tokens.json";
        private readonly Encoding encoding = Encoding.UTF8;
        private const string refreshTokenString = "refreshToken";

        public TokenStore() { }

        public TokenStore(string storePath)
        {
            StorePath = storePath;
        }

        public string? GetRefreshToken()
        {
            var data = ReadJsonFile(StorePath);
            if (data?.ContainsKey(refreshTokenString) == true)
            {
                var tokenBytes = Convert.FromBase64String(data[refreshTokenString]);
                return encoding.GetString(tokenBytes);
            }
            return null;
        }

        public void SaveRefreshToken(string token)
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

        private Dictionary<string,string>? ReadJsonFile(string path)
        {
            if (!File.Exists(path))
                return null;

            var stream = File.OpenRead(path);
            return JsonSerializer.Deserialize<Dictionary<string, string>>(stream);
        }
    }
}
