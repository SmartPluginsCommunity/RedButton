using Microsoft.VisualStudio.TestTools.UnitTesting;
using RedButton.Common.TeklaStructures.Extensions;
using System.Linq;
using Tekla.Structures.Geometry3d;

namespace RedButton.Tests.UnitTests
{
    [TestClass]
    public class SolidExtensionTests : BaseTest
    {
        [TestMethod("Get intersect with beam")]
        public void GetAllPartFromAssemblyTest()
        {
            var beam1 = TestObjectCreator.GetBeam();

            AddTemporaryObject(beam1);

            var p1 = new Point(500, 0, 0);
            var p2 = new Point(500, 0, 1000);
            var p3 = new Point(500, 1000, 0);

            var intersect = beam1.GetSolid().GetAllContours(p1, p2, p3);

            Assert.IsNotNull(intersect.OuterContour);
        }
    }
}
