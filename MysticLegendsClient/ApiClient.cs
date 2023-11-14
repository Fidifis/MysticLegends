using MysticLegendsShared.Utilities;
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
        private bool disposed = false;

        private string accessToken = "";
        public string AccessToken { get => accessToken;
            set
            {
                accessToken = value;
                client.DefaultRequestHeaders.Remove("access-token");
                client.DefaultRequestHeaders.Add("access-token", value);
            }
        }

        private readonly HttpClient client = new();

        public string Host { get; private init; }

        public async Task<bool> HealthCheckAsync()
        {
            try
            {
                var status = await GetAsync<Dictionary<string, string>>("api/Health");
                return status.Get("status") == "ok";
            }
            catch (Exception) { }

            return false;
        }

        public ApiClient(string address, TimeSpan? timeout = null)
        {
            Host = address;

            client.BaseAddress = new Uri(Host);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            client.Timeout = timeout ?? TimeSpan.FromSeconds(20);
        }

        //private static Dictionary<string, string> AppendToken(IReadOnlyDictionary<string, string>? paramters)
        //{
        //    Dictionary<string, string> dict = paramters is not null ? new(paramters) : new();
        //    dict["accessToken"] = "lol";
        //    return dict;
        //}

        public async Task<T> GetAsync<T>(string path, IReadOnlyDictionary<string,string>? parameters = null)
        {
            ObjectDisposedException.ThrowIf(disposed, this);

            path = path.TrimEnd('/');
            var combined = path;

            int i = 0;
            if (parameters is not null)
                foreach (var param in parameters)
                {
                    combined += i++ == 0 ? "?" : "&";
                    combined += $"{param.Key}={param.Value}";
                }

            using var response = await client.GetAsync(combined);
            if (!response.IsSuccessStatusCode)
                throw new NetworkException(await response.Content.ReadAsStringAsync());

            return await response.Content.ReadFromJsonAsync<T?>() ?? throw new NullReferenceException();
        }

        public async Task<T> PostAsync<T>(string path, IReadOnlyDictionary<string, string>? parameters = null)
        {
            ObjectDisposedException.ThrowIf(disposed, this);

            using var response = await client.PostAsJsonAsync(path, parameters);
            if (!response.IsSuccessStatusCode)
                throw new NetworkException($"HTTP code: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");

            return await response.Content.ReadFromJsonAsync<T?>() ?? throw new NullReferenceException();
        }

        public void Dispose()
        {
            if (disposed)
                return;

            client.Dispose();
            disposed = true;
        }
    }
}
