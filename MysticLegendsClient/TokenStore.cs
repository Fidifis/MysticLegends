using MysticLegendsShared.Utilities;
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

        public async Task<string?> ReadRefreshTokenAsync(string host)
        {
            var data = await ReadFromJsonAsync(StorePath, host);
            if (data?.ContainsKey(refreshTokenString) == true)
            {
                var tokenBytes = Convert.FromBase64String(data[refreshTokenString]);
                return encoding.GetString(tokenBytes);
            }
            return null;
        }

        public async Task<string?> ReadUserNameAsync(string host)
        {
            var data = await ReadFromJsonAsync(StorePath, host);
            if (data?.ContainsKey(usernameString) == true)
            {
                return data[usernameString];
            }
            return null;
        }

        public async Task SaveRefreshToken(string? token, string host)
        {
            var dir = Path.GetDirectoryName(StorePath);

            EnsureSavePath(dir!);

            var data = await ReadFromJsonAsync(StorePath, host);

            data?.Remove(refreshTokenString);
            data ??= new();

            if (token is not null)
            {
                var tokenBytes = encoding.GetBytes(token);
                var base64Token = Convert.ToBase64String(tokenBytes);

                data[refreshTokenString] = base64Token;
            }

            await WriteToJsonAsync(data, StorePath, host);
        }

        public async Task SaveUsername(string? username, string host)
        {
            var dir = Path.GetDirectoryName(StorePath);

            EnsureSavePath(dir!);

            var data = await ReadFromJsonAsync(StorePath, host);
            data?.Remove(usernameString);
            data ??= new();

            if (username is not null)
            {
                data[usernameString] = username;
            }

            await WriteToJsonAsync(data, StorePath, host);
        }

        private async Task<Dictionary<string,string>?> ReadFromJsonAsync(string path, string host)
        {
            if (!File.Exists(path))
                return null;

            using var stream = File.OpenRead(path);
            var hostContext = await JsonSerializer.DeserializeAsync<Dictionary<string, Dictionary<string, string>>>(stream);
            return hostContext?.Get(host);
        }

        private async Task WriteToJsonAsync(Dictionary<string, string>? data, string path, string host)
        {
            var hostContext = new Dictionary<string, Dictionary<string, string>>();

            if (data is null)
                hostContext.Remove(host);
            else
                hostContext[host] = data;

            var json = JsonSerializer.Serialize(hostContext);
            await File.WriteAllTextAsync(StorePath, json);
        }

        private void EnsureSavePath(string dir)
        {
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir!);
        }
    }
}
