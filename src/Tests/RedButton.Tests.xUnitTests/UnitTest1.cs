using Microsoft.VisualStudio.TestTools.UnitTesting;
using RedButton.Common.TeklaStructures.Extensions;

namespace RedButton.Tests.UnitTests
{
    [TestClass]
    public class UnitTest1 : BaseTest
    {
        [TestMethod("Visible test name")]
        public void TestMethod1()
        {
            var mos = new Tekla.Structures.Model.UI.ModelObjectSelector();

            mos.GetSelectedObjects();
        }
    }
}
