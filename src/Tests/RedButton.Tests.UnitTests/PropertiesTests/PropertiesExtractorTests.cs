using Microsoft.VisualStudio.TestTools.UnitTesting;
using RedButton.Common.TeklaStructures.Model.Properties;

namespace RedButton.Tests.UnitTests.PropertiesTests
{
    [TestClass]
    public class PropertiesExtractorTests : BaseTest
    {
        [TestMethod("Get several properties")]
        public void GetSeveralPropertiesTest()
        {
            var beam1 = TestObjectCreator.GetBeam();

            AddTemporaryObject(beam1);

            var properties = new PropertiesExtractor(beam1);

            var width = properties.AddDoubleAttribute("WIDTH");
            var lenght = properties.AddDoubleAttribute("LENGTH");
            var name = properties.AddStringAttribute("NAME");
            var total = properties.AddIntegerAttribute("MODEL_TOTAL");

            properties.ExtractProperties();

            Assert.IsNotNull(width);
            Assert.IsNotNull(lenght);
            Assert.IsNotNull(name);
            Assert.IsNotNull(total);
        }
    }
}
