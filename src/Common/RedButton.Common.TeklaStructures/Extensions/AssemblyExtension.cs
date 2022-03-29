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
        /// <param name="assembly">Assembly</param>
        /// <param name="getMainPart">Get main part.</param>
        /// <returns></returns>
        public static IEnumerable<T> GetAllParts<T>(this Assembly assembly, bool getMainPart) where T : ModelObject
        {
            var result = new List<T>();

            if (getMainPart)
            {
                if (assembly.GetMainPart() is T mainPart)
                    result.Add(mainPart);
            }

            foreach (var obj in assembly.GetSecondaries())
            {
                if(obj is T modelObject)
                    result.Add(modelObject);
            }

            return result;
        }
    }
}
