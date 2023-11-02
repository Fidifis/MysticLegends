using System.Collections.Immutable;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace MysticLegendsClient
{
    internal class NetworkException : Exception
    {
        public NetworkException(string message) : base(message) { }
    }

    internal class ApiClient: IDisposable
    {
        private readonly HttpClient client = new();

        public async Task<bool> HealthCheckAsync()
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

            return await response.Content.ReadFromJsonAsync<T?>();
        }

        public async Task<T?> PostAsync<T>(string path, IImmutableDictionary<string, string>? parameters = null)
        {
            parameters = AppendToken(parameters);

            using var response = await client.PostAsJsonAsync(path, parameters);
            if (!response.IsSuccessStatusCode)
                return default;

            return await response.Content.ReadFromJsonAsync<T?>();
        }
    }
}
