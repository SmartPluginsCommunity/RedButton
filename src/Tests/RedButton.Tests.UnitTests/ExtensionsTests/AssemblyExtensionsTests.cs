using Microsoft.VisualStudio.TestTools.UnitTesting;
using RedButton.Common.TeklaStructures.Extensions;
using System.Collections.Generic;
using System.Linq;
using Tekla.Structures.Model;

namespace RedButton.Tests.UnitTests
{
    [TestClass]
    public class GetPartsTests : BaseTest
    {
        [TestMethod("Get all part from assembly")]
        public void GetAllPartFromAssemblyTest()
        {
            var beam1 = GetBeam();
            var beam2 = GetBeam();
            var beam3 = GetBeam();

            var assembly = CreateAssembly(beam1, new List<Part> { beam2, beam3 });

            var allParts = assembly.GetAllParts<Part>(true);

            Assert.AreEqual(allParts.Count(), 3);
        }

        [TestMethod("Get all part from assembly without main part")]
        public void GetAllPartFromAssemblyWithoutMainPartTest()
        {
            var beam1 = GetBeam();
            var beam2 = GetBeam();
            var beam3 = GetBeam();

            var assembly = CreateAssembly(beam1, new List<Part> { beam2, beam3 });

            var allParts = assembly.GetAllParts<Part>(false);

            Assert.AreEqual(allParts.Count(), 2);
        }
    }
}
