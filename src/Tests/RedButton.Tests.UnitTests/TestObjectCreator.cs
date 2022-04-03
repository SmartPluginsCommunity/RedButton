using System.Collections.Generic;
using Tekla.Structures.Geometry3d;
using Tekla.Structures.Model;

namespace RedButton.Tests.UnitTests
{
    public class TestObjectCreator
    {
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

        public Assembly CreateAssembly(Part mainPart, IEnumerable<Part> secondaries)
        {
            var assembly = mainPart.GetAssembly();

            foreach (var part in secondaries)
                assembly.Add(part);

            return assembly;
        }
    }
}
