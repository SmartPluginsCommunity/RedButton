using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace RedButton.Common.Core.CollectionExtensions
{
    public static class EnumeratorExtensions
    {
        /// <summary>
        /// IEnumerator to IEnumerable.
        /// </summary>
        /// <param name="enumerator"></param>
        /// <returns></returns>
        public static IEnumerable<T> ToIEnumerable<T>(this IEnumerator enumerator)
        {
            while (enumerator.MoveNext())
            {
                if (enumerator.Current is T item)
                    yield return item;
            }
        }

        /// <summary>
        /// IEnumerator to IList.
        /// </summary>
        /// <param name="enumerator"></param>
        /// <returns></returns>
        public static IReadOnlyList<T> ToList<T>(this IEnumerator enumerator)
        {
            var result = new List<T>();
            
            while (enumerator.MoveNext())
            {
                if (enumerator.Current is T item)
                    result.Add(item);
            }

            return result;
        }

        /// <summary>
        /// IEnumerator to ConcurrentBag.
        /// </summary>
        /// <param name="enumerator"></param>
        /// <returns></returns>
        public static ConcurrentBag<T> ToConcurrentBag<T>(this IEnumerator enumerator)
        {
            return new ConcurrentBag<T>(enumerator.ToIEnumerable<T>());
        }
    }
}
