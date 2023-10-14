using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;

namespace MysticLegendsClient
{
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

        public async Task<T?> GetAsync<T>(string path, params string[] arguments)
        {
            var combinedPath = arguments.Length > 0 ?
                string.Join("/", path.TrimEnd('/'), arguments) :
                path;

            var response = await client.GetAsync(combinedPath);
            if (!response.IsSuccessStatusCode)
                return default;

            var json = await response.Content.ReadAsStreamAsync();
            return await JsonSerializer.DeserializeAsync<T>(json);
        }
    }
}
