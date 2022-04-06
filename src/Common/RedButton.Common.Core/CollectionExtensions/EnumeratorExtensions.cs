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
        /// <typeparam name="T"></typeparam>
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
        /// IEnumerator to ConcurrentBag.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerator"></param>
        /// <returns></returns>
        public static ConcurrentBag<T> ToConcurrentBag<T>(this IEnumerator enumerator)
        {
            return new ConcurrentBag<T>(enumerator.ToIEnumerable<T>());
        }
    }
}
