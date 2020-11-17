using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LeanCode.Standard.TypeExt
{
    public static class EnumerableExt
    {
        public static IEnumerable<IEnumerable<T>> Batch<T>(this IEnumerable<T> list, int batchSize)
        {
            while (list.Any())
            {
                var batch = list.Take(batchSize);

                list = list.Skip(batchSize);

                yield return batch;
            }
        }

        public static bool IsNullOrEmpty(this IEnumerable source)
        {
            if (source == null)
            {
                return true;
            }

            foreach (object obj in source)
            {
                return false;
            }

            return true;
        }

        public static bool IsNullOrEmpty<T>(this IEnumerable<T> source)
        {
            if (source == null)
            {
                return true;
            }

            return !source.Any();
        }

        public static List<T> ToNullSafeList<T>(this IEnumerable<T> source)
        {
            if (source.IsNullOrEmpty())
            {
                return new List<T>();
            }

            return source.ToList();
        }

        public static IEnumerable<T> OrEmptyIfNull<T>(this IEnumerable<T> source)
        {
            return source ?? Enumerable.Empty<T>();
        }

        public static Dictionary<TKey, TItem> ToSafeDictionary<TItem, TKey>(this IEnumerable<TItem> items, Func<TItem, TKey> keyProjection)
        {
            return
                ToSafeDictionary<TKey,TItem>(items.Where(x=> keyProjection(x)!=null).
                Select(item => new KeyValuePair<TKey, TItem>(keyProjection(item), item)));
        }

        public static Dictionary<TKey, TValue> ToSafeDictionary<TItem, TKey, TValue>(this IEnumerable<TItem> items, Func<TItem, TKey> keyProjection, Func<TItem, TValue> valueProjection)
        {
            return
                ToSafeDictionary(items.Select(item => new KeyValuePair<TKey, TValue>(keyProjection(item), valueProjection(item))));
        }

        public static Dictionary<TKey, TValue> ToSafeDictionary<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> items)
        {
            var map = new Dictionary<TKey, TValue>();
            foreach (var keyValuePair in items)
            {
                map[keyValuePair.Key] = keyValuePair.Value;
            }
            return map;
        }

        public static IEnumerable<IEnumerable<T>> BreakIntoChunks<T>(this IEnumerable<T> allItems, int chunkSize)
        {
            for (int i = 0; i < allItems.Count(); i += chunkSize)
                yield return allItems.Skip(i).Take(chunkSize);
        }
    }
}
