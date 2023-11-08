namespace MysticLegendsShared.Utilities;

public static class Extensions
{
    public static TValue? Get<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dict, TKey key) => dict.ContainsKey(key) ? dict[key] : default;

    public static T? Get<T>(this IList<T> list, int? index)
    {
        if (index is null)
            return default;

        if (list.Count <= index)
            return default;

        return list[(int)index];
    }

    public static IReadOnlyCollection<T> AsReadOnly<T>(this ICollection<T> collection) => (IReadOnlyCollection<T>)collection;
}
