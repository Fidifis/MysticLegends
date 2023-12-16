using MysticLegendsShared.Utilities;
using System.IO;
using System.Text.Json;

namespace MysticLegendsClient;

// TODO: use serialization
internal class ConfigStore
{
    public readonly string StorePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Mystic Legends\config.json";

    public ConfigStore() { }

    public ConfigStore(string storePath)
    {
        StorePath = storePath;
    }

    public async Task<string?> ReadAsync(string key)
    {
        if (!File.Exists(StorePath))
            return null;

        using var stream = File.OpenRead(StorePath);
        var data = await JsonSerializer.DeserializeAsync<Dictionary<string, string>>(stream);

        return data?.Get(key);
    }

    public async Task WriteAsync(string key, string? value)
    {
        EnsureSavePath();

        Dictionary<string, string>? data = null;
        if (File.Exists(StorePath))
        {
            using var stream = File.OpenRead(StorePath);
            data = await JsonSerializer.DeserializeAsync<Dictionary<string, string>>(stream) ?? new();
            stream.Close();
        }
        data ??= new();

        if (value is null)
            data.Remove(key);
        else
            data[key] = value;

        var jsonSerializerOptions = new JsonSerializerOptions
        {
            WriteIndented = true
        };

        var json = JsonSerializer.Serialize(data, jsonSerializerOptions);
        await File.WriteAllTextAsync(StorePath, json);
    }

    private void EnsureSavePath()
    {
        var dir = Path.GetDirectoryName(StorePath);
        if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir!);
    }
}
