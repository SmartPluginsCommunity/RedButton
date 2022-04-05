using System.Collections.Generic;
using RedButton.Common.Core.Geometry;
using Xunit;

namespace RedButton.Tests.xUnitTests.RedButton.Common.Core.Geometry
{
    public class VectorTest
    {
        [Theory]
        [MemberData(nameof(GetVectorVectorProductData))]
        public void GetVectorProduct(Vector v1, Vector v2, Vector resultVector)
        {
            var result = v1.VectorProduct(v2);
            Assert.True(resultVector == result);
        }
        
        [Theory]
        [MemberData(nameof(GetVectorAngleData))]
        public void GetAngle(Vector v1, Vector v2, double resultAngle)
        {
            double result = v1.GetAngleInDegree(v2);
            Assert.True(resultAngle == result);
        }
        
        public static IEnumerable<object[]> GetVectorAngleData()
        {
            yield return new object[]
            {
                new Vector(1, 0, 0),
                new Vector(0, 1, 0),
                90.0,
            };
            yield return new object[]
            {
                new Vector(1,0,0),
                new Vector(1, 0, 0),
                0.0,
            };
            yield return new object[]
            {
                new Vector(1,0,0),
                new Vector(-1, 0, 0),
                180.0,
            };
            yield return new object[]
            {
                new Vector(1,1,0),
                new Vector(-1, 0, 0),
                135.0,
            };
        }

        public static IEnumerable<object[]> GetVectorVectorProductData()
        {
            yield return new object[]
            {
                new Vector(1, 0, 0),
                new Vector(0, 1, 0),
                new Vector(0, 0, 1)
            };
            yield return new object[]
            {
                new Vector(-1, 0, 0),
                new Vector(0, 1, 0),
                new Vector(0, 0, -1)
            };
        }

    }
}