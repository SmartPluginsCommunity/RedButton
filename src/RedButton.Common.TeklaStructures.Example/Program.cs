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
            DrawingsUtil();
            Console.Read();
        }

        public static void DrawingsUtil()
        {
            var drawTable = new DrawTable();
           var drawings =  DrawingUtils.GetSelectedDrawings();
           if (drawings.Count > 0)
           {
               Console.WriteLine(drawings.Count);
               Console.WriteLine(drawings[0].Name);
           }
           else
           {
               Console.WriteLine("Нет чертежей");
           }
        }

    }
}
