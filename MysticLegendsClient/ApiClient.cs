using System.Collections.Immutable;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
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

        private static IImmutableDictionary<string, string> AppendToken(IImmutableDictionary<string, string>? paramters)
        {
            var builder = ImmutableDictionary.CreateBuilder<string, string>();
            if (paramters is not null) builder.AddRange(paramters);
            builder["accessToken"] = "lol";
            return builder.ToImmutable();
        }

        public async Task<T?> GetAsync<T>(string path, IImmutableDictionary<string,string>? parameters = null)
        {
            parameters = AppendToken(parameters);
            path = path.TrimEnd('/');
            var combined = path;

            int i = 0;
            foreach (var param in parameters)
            {
                combined += i++ == 0 ? "?" : "&";
                combined += $"{param.Key}={param.Value}";
            }

            using var response = await client.GetAsync(combined);
            if (!response.IsSuccessStatusCode)
                return default;

            return await response.Content.ReadFromJsonAsync<T>();
        }

        public async Task<T?> PostAsync<T>(string path, IImmutableDictionary<string, string>? parameters = null)
        {
            parameters = AppendToken(parameters);

            using var response = await client.PostAsJsonAsync(path, parameters);
            if (!response.IsSuccessStatusCode)
                return default;

            return await response.Content.ReadFromJsonAsync<T>();
        }
    }
}
