using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RedButton.Common.TeklaStructures.DrawingTable;

namespace RedButton.Common.TeklaStructures.Example
{
    internal class Program
    {
        static void Main(string[] args)
        {
            DrawListOfDrawingsTable();
            Console.Read();
        }

        public static void DrawListOfDrawingsTable()
        {
            var listOfDrawingsTable = new ListOfDrawingsTable();
           
            //Selected drawings in Tekla
            var drawings = DrawingUtils.GetSelectedDrawings();
            //var drawings = DrawingUtils.GetDrawings();

            //Insert table on drawing in Tekla
            listOfDrawingsTable.CreateTable(drawings);
        }

    }
}
