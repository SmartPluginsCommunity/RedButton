using Microsoft.VisualStudio.TestTools.UnitTesting;
using RedButton.Common.TeklaStructures.DrawingTable;

namespace RedButton.Tests.UnitTests.DrawingTests
{
    [TestClass]
    public class DrawingUtilsTests : BaseTest
    {
        [TestMethod("Test table create [NOT AUTO]")]
        public void TableCreateTest()
        {
            var listOfDrawingsTable = new ListOfDrawingsTable();

            //Table of all drawings
            var drawings = DrawingUtils.GetDrawings();

            //Table of selected drawings in Tekla
            //var drawings = DrawingUtils.GetSelectedDrawings();

            //Insert table on drawing in Tekla
            listOfDrawingsTable.CreateTable(drawings);
        }
    }
}
