using System;
using NUnit.Framework;
using RedButton.Common.TeklaStructures.Extensions;
using Tekla.Structures.Geometry3d;

namespace RedButton.Tests.UnitTests
{
    public class TestVectorExtensions
    {
        private const double TOLERANCE = 0.001;
        [Test]
        public void ПроверкаОртогональностиВекторов()
        {
            //ARRANGE
            var v1 = new Vector(1897.6551, 1394.7484, 0.0);
            var vOrtho  = v1.GetOrthoVector2d();
            var angle = v1.GetAngleBetween(vOrtho)* 180 / Math.PI;
            var b = angle -90 <TOLERANCE; 
            Assert.AreEqual(true, b);
        }
        
        [Test]
        public void ПроверкаОртогональностиВекторов1()
        {
            //ARRANGE
            var v1 = new Vector(1, 0, 0.0);
            var vOrtho  = v1.GetOrthoVector2d();
            var angle = v1.GetAngleBetween(vOrtho)* 180 / Math.PI;
            var b = angle -90 <TOLERANCE; 
            Assert.AreEqual(true, b);
        }
        
        [Test]
        public void ПроверкаОртогональностиВекторов2()
        {
            //ARRANGE
            var v1 = new Vector(0, 1, 0.0);
            var vOrtho  = v1.GetOrthoVector2d();
            var angle = v1.GetAngleBetween(vOrtho)* 180 / Math.PI;
            var b = angle -90 <TOLERANCE; 
            Assert.AreEqual(true, b);
        }
    }
}