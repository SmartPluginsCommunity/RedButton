using System.Collections.Generic;
using Tekla.Structures.Geometry3d;
using Tekla.Structures.Model;

namespace RedButton.Tests.UnitTests
{
    /// <summary>
    /// Create model objects for tests
    /// </summary>
    public class TestObjectCreator
    {
        /// <summary>
        /// Get beam
        /// </summary>
        /// <returns></returns>
        public Beam GetBeam()
        {
            var beam = new Beam();
            beam.Profile.ProfileString = "D100";
            beam.StartPoint = new Point();
            beam.EndPoint = new Point(1000, 0, 0);

            if (beam.Insert())
                return beam;

            return null;
        }

        /// <summary>
        /// Create assembly
        /// </summary>
        /// <param name="mainPart"></param>
        /// <param name="secondaries"></param>
        /// <returns></returns>
        public Assembly CreateAssembly(Part mainPart, IEnumerable<Part> secondaries)
        {
            var assembly = mainPart.GetAssembly();

            foreach (var part in secondaries)
                assembly.Add(part);

            return assembly;
        }
    }
}
