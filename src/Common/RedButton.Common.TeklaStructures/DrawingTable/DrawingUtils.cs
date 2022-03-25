using System;
using System.Collections.Generic;
using Tekla.Structures.Drawing;

namespace RedButton.Common.TeklaStructures.DrawingTable
{
    public class DrawingUtils
    {
        /// <summary>
        /// Get current drawing title.
        /// </summary>
        /// <returns>Current drawing title</returns>
        public static string GetCurrentDrawingTitle()
        {
            var currentDrawingName = string.Empty;

            var drawHand = new DrawingHandler();

            if (drawHand.GetConnectionStatus())
            {
                var drawing = drawHand.GetActiveDrawing();
                currentDrawingName = drawing.Title1 + " " + drawing.Title2 + " " + drawing.Title3;
            }

            return currentDrawingName;
        }

        /// <summary>
        /// Get drawing title.
        /// </summary>
        /// <param name="drawing">Existing drawing.</param>
        /// <returns>Drawing title</returns>
        public static string GetDrawingTitle(Drawing drawing)
        {
            var currentDrawingName = drawing.Title1 + " " + drawing.Title2 + " " + drawing.Title3;
            return currentDrawingName;
        }

        /// <summary>
        /// Get drawings titles
        /// </summary>
        /// <param name="drawings">List of drawings</param>
        /// <returns>Drawings titles</returns>
        public static IEnumerable<string> GetDrawingsTitles(IEnumerable<Drawing> drawings)
        {
            var titles = new List<string>();
            foreach (var drawing in drawings)
            {
                titles.Add(GetDrawingTitle(drawing));
            }

            return titles;
        }

        /// <summary>
        /// Get drawings.
        /// </summary>
        /// <returns>List of drawings</returns>
        public static IEnumerable<Drawing> GetDrawings()
        {
            var drawings = new List<Drawing>();

            var drawHand = new DrawingHandler();

            if (drawHand.GetConnectionStatus())
            {
                var drawEnum = drawHand.GetDrawings();

                foreach (Drawing drawing in drawEnum)
                {
                    drawings.Add(drawing);
                }
            }

            return drawings;
        }

        /// <summary>
        /// Get selected drawings
        /// </summary>
        /// <returns>Selected drawings</returns>
        public static IEnumerable<Drawing> GetSelectedDrawings()
        {
            var drawings = new List<Drawing>();

            var drawHand = new DrawingHandler();

            if (drawHand.GetConnectionStatus())
            {
                var drawEnum = drawHand.GetDrawingSelector().GetSelected();

                foreach (Drawing drawing in drawEnum)
                {
                    drawings.Add(drawing);
                }
            }

            return drawings;
        }

        /// <summary>
        /// Get sheets numbers array.
        /// </summary>
        /// <param name="drawings">List of drawing.</param>
        /// <returns>Sheets numbers array.</returns>
        public static double[] SheetNumbers(List<Drawing> drawings)
        {
            var sheetNumbers = new double[drawings.Count];
            var sheetNumberAtr = string.Empty;
            int i = 0;

            foreach (var drawing in drawings)
            {
                try
                {
                    drawing.GetUserProperty("ru_list", ref sheetNumberAtr);
                    sheetNumbers[i] = 0;
                    Double.TryParse(sheetNumberAtr, out sheetNumbers[i]);
                }
                catch
                {
                    sheetNumbers[i] = 0;
                }

                i++;
            }

            return sheetNumbers;
        }
    }
}