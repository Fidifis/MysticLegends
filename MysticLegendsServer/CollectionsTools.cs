using System.Collections.Immutable;

namespace MysticLegendsServer
{
    public static class CollectionsTools
    {
        public static async Task<List<T>> ConstructList<T>(this IAsyncEnumerable<T> list)
        {
            var newList = new List<T>();
            await foreach (var item in list)
            {
                newList.Add(item);
            }
            return newList;
        }
        public static List<T> ConstructList<T>(this IEnumerable<T> list)
        {
            var newList = new List<T>();
            foreach (var item in list)
            {
                newList.Add(item);
            }
            return newList;
        }
        public static async Task<ImmutableList<T>> ConstructImmutableList<T>(this IAsyncEnumerable<T> list)
        {
            var builder = ImmutableList.CreateBuilder<T>();
            await foreach (var item in list)
            {
                builder.Add(item);
            }
            return builder.ToImmutable();
        }
        public static ImmutableList<T> ConstructImmutableList<T>(this IEnumerable<T> list)
        {
            var builder = ImmutableList.CreateBuilder<T>();
            foreach (var item in list)
            {
                builder.Add(item);
            }
            return builder.ToImmutable();
        }
    }
}
