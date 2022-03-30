using System.Collections.Generic;
using Tekla.Structures.Model;

namespace RedButton.Common.TeklaStructures.Extensions
{
    public static class AssemblyExtension
    {
        /// <summary>
        /// Get all parts for assembly.
        /// </summary>
        /// <param name="assembly">Assembly</param>
        /// <param name="getMainPart">Get a main part</param>
        /// <returns></returns>
        public static IEnumerable<T> GetAllParts<T>(this Assembly assembly, bool getMainPart) where T : ModelObject
        {
            if (getMainPart)
            {
                if (assembly.GetMainPart() is T mainPart)
                    yield return mainPart;
            }

            var secondaries = assembly.GetSecondaries();

            foreach (var obj in secondaries)
            {
                if(obj is T modelObject)
                    yield return modelObject;
            }
        }
    }
}
