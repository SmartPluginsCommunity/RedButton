using Microsoft.VisualStudio.TestTools.UnitTesting;
using RedButton.Common.Core.Geometry;

namespace RedButton.Tests.UnitTests.RedButton.Common.Core.Geometry
{
    [TestClass]
    public class SphereTest : BaseTest
    {
        /// <summary>
        /// Initial logic
        /// </summary>
        [TestMethod("Test intersection")]
        public void IntersectionTest()
        {
            var sphere1 = new Sphere(new Point(), 10);
            var sphere2 = new Sphere(new Point(5), 10);

            var result = sphere1.IsIntersectionSphere(sphere2);
            
            Assert.IsTrue(result);
        }
    }
}