using System;
using RedButton.Common.TeklaStructures.DrawingTable;
using RedButton.Common.TeklaStructures.Dim;

namespace RedButton.Common.TeklaStructures.Example
{
    internal class Program
    {
        static void Main(string[] args)
        {
            DrawTable();
            Console.Read();
        }

        public static void DrawTable()
        {
            var listOfDrawingsTable = new ListOfDrawingsTable();

            //Table of all drawings
            var drawings = DrawingUtils.GetDrawings();

            //Table of selected drawings in Tekla
            //var drawings = DrawingUtils.GetSelectedDrawings();

            //Insert table on drawing in Tekla
            listOfDrawingsTable.CreateTable(drawings);
        }

        public void SimpleDimension()
        {
            var dim = new SimpleDimension();
            dim.SimpleSectionDimension();
        }
    }
}