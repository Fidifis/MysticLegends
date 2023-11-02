namespace MysticLegendsShared.Utilities;

public static class Extensions
{
    public static TValue? Get<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dict, TKey key) => dict.ContainsKey(key) ? dict[key] : default;
}
