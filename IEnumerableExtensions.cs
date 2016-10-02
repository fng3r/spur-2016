using System;
using System.Collections.Generic;

namespace SpurRoguelike.PlayerBot
{
    static class LinqExtensions
    {
        public static TSource MinBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector,
            IComparer<TKey> comparer)
        {
            using (var sourceIterator = source.GetEnumerator())
            {
                if (!sourceIterator.MoveNext())
                    throw new InvalidOperationException("A sequence have no elements");

                var min = sourceIterator.Current;
                var minKey = selector(min);

                while (sourceIterator.MoveNext())
                {
                    var candidate = sourceIterator.Current;
                    var currentKey = selector(candidate);
                    if (comparer.Compare(currentKey, minKey) < 0)
                    {
                        min = candidate;
                        minKey = currentKey;
                    }
                }
                return min;
            }
        }

        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> source)
        {
            return new HashSet<T>(source);
        }
    }

}
