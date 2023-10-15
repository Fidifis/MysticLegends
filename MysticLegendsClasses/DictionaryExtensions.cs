namespace MysticLegendsClasses
{
    public static class DictionaryExtensions
    {
        public static TValue? Get<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key) => dict.ContainsKey(key) ? dict[key] : default;
    }
}
