using System.Collections;

namespace RedButton.Common.Core.CollectionExtensions
{
    public static class EnumerableExtensions
    {
        /// <summary>
        /// IEnumerable to ArrayList
        /// </summary>
        /// <param name="enumerable">IEnumerable</param>
        /// <returns></returns>
        public static ArrayList ToArrayList(this IEnumerable enumerable)
        {
            var array = new ArrayList();

            foreach (var item in enumerable)
            {
                if (item != null)
                    array.Add(item);
            }

            return array;
        }
    }
}
