using System.Collections.Generic;
using System.Linq;
using Tekla.Structures.Model;

namespace RedButton.Common.TeklaStructures.Extensions
{
    public static class AssemblyExtension
    {
        /// <summary>
        /// Get all parts for assembly.
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="getMainPart">Надо ли получать главную деталь сборки.</param>
        /// <returns></returns>
        public static IEnumerable<T> GetAllParts<T>(this Assembly assembly, bool getMainPart) where T : ModelObject
        {
            var result = new List<T>();

            if (getMainPart)
                result.Add(assembly.GetMainPart() as T);

            var secondariesParts = assembly.GetSecondaries().OfType<T>();
            result.AddRange(secondariesParts);

            return result.Where(part => part != null);
        }
    }
}
