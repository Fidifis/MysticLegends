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
        private const string usernameString = "username";

        public TokenStore() { }

        public TokenStore(string storePath)
        {
            StorePath = storePath;
        }

        public async Task<string?> ReadRefreshTokenAsync()
        {
            var data = await ReadJsonFileAsync(StorePath);
            if (data?.ContainsKey(refreshTokenString) == true)
            {
                var tokenBytes = Convert.FromBase64String(data[refreshTokenString]);
                return encoding.GetString(tokenBytes);
            }
            return null;
        }

        public async Task<string?> ReadUserNameAsync()
        {
            var data = await ReadJsonFileAsync(StorePath);
            if (data?.ContainsKey(usernameString) == true)
            {
                return data[usernameString];
            }
            return null;
        }

        public async Task SaveRefreshToken(string? token)
        {
            var dir = Path.GetDirectoryName(StorePath);

            EnsureSavePath(dir!);

            var data = await ReadJsonFileAsync(StorePath) ?? new();

            data.Remove(refreshTokenString);

            if (token is not null)
            {
                var tokenBytes = encoding.GetBytes(token);
                var base64Token = Convert.ToBase64String(tokenBytes);

                data[refreshTokenString] = base64Token;
            }

            var json = JsonSerializer.Serialize(data);
            await File.WriteAllTextAsync(StorePath, json);
        }

        public async Task SaveUsername(string? username)
        {
            var dir = Path.GetDirectoryName(StorePath);

            EnsureSavePath(dir!);

            var data = await ReadJsonFileAsync(StorePath) ?? new();

            data.Remove(usernameString);

            if (username is not null)
            {
                data[usernameString] = username;
            }

            var json = JsonSerializer.Serialize(data);
            await File.WriteAllTextAsync(StorePath, json);
        }

        private async Task<Dictionary<string,string>?> ReadJsonFileAsync(string path)
        {
            if (!File.Exists(path))
                return null;

            using var stream = File.OpenRead(path);
            return await JsonSerializer.DeserializeAsync<Dictionary<string, string>>(stream);
        }

        private void EnsureSavePath(string dir)
        {
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir!);
        }
    }
}
