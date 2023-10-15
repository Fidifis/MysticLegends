using System.Collections.Immutable;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;

namespace MysticLegendsClient
{
    internal class NetworkException : Exception
    {
        public NetworkException(string message): base(message) { }
    }

    internal class ApiClient: IDisposable
    {
        public static ApiClient? Connection = null;
        private readonly HttpClient client = new();

        public static async Task Connect(string address)
        {
            if (Connection is not null)
                throw new InvalidOperationException("Connection already established");

            var newClient = new ApiClient(address);
            if (!await newClient.CheckServerStatusAsync())
                throw new HttpRequestException("Connection failed");

            Connection = newClient;
        }

        private async Task<bool> CheckServerStatusAsync()
        {
            var status = await GetAsync<Dictionary<string, string>>("api/Health");
            try
            {
                return status?["status"] == "ok";
            }
            catch (Exception)
            {
                return false;
            }
        }

        public ApiClient(string address)
        {
            client.BaseAddress = new Uri(address);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public static void Disconnect()
        {
            if (Connection is null)
                return;

            Connection.Dispose();
            Connection = null;
        }

        public void Dispose()
        {
            client.Dispose();
        }

        public async Task<T?> GetAsync<T>(string path, params KeyValuePair<string,string>[] parameters)
        {
            path = path.TrimEnd('/');
            var combined = path;
            for (int i = 0; i < parameters.Length; i++)
            {
                combined += i == 0 ? "?" : "&";
                combined += $"{parameters[i].Key}={parameters[i].Value}";
            }

            var response = await client.GetAsync(combined);
            if (!response.IsSuccessStatusCode)
                return default;

            var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
            var json = await response.Content.ReadAsStreamAsync();
            return await JsonSerializer.DeserializeAsync<T>(json, options);
        }
    }
}
