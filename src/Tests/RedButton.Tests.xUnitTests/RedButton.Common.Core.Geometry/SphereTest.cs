using System.Collections.Generic;
using RedButton.Common.Core.Geometry;
using Xunit;

namespace RedButton.Tests.xUnitTests.RedButton.Common.Core.Geometry
{
    public class ShpereTest
    {
        [Theory]
        [MemberData(nameof(GetSphereData))]
        public void IntersectionTest(Sphere sphere1, Sphere sphere2, bool resultData)
        {
            var result = sphere1.IsIntersectionSphere(sphere2);
            
            Assert.Equal(result, resultData);
        }

        public static IEnumerable<object[]> GetSphereData()
        {
            yield return new object[]
            {
                new Sphere(new Point(), 10), 
                new Sphere(new Point(5), 10), 
                true
            };
            yield return new object[]
            {
                new Sphere(new Point(), 10), 
                new Sphere(new Point(10), 10), 
                true
            };
            yield return new object[]
            {
                new Sphere(new Point(), 10), 
                new Sphere(new Point(25), 10), 
                false
            };
        }
    }
}