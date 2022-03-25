using System;
using System.Collections.Generic;
using System.Linq;
using Tekla.Structures.Geometry3d;
using TSD = Tekla.Structures.Drawing;


namespace RedButton.Common.TeklaStructures.DrawingTable
{
    /// <summary>
    ///     Create List Of drawing table.
    /// </summary>
    public class ListOfDrawingsTable
    {
        /// <summary>
        /// Create table of drawings names.
        /// </summary>
        /// <param name="drawings">List of drawings</param>
        /// <param name="headerTitle"></param>
        public void CreateTable(IEnumerable<TSD.Drawing> drawings,
            string headerTitle = "Ведомость рабочих чертежей основного комплекта")
        {
            var orderedDrawings = drawings.OrderBy(x =>
                {
                    var number = string.Empty;
                    x.GetUserProperty("ru_list", ref number);
                    double numberDouble = 0;
                    double.TryParse(number, out numberDouble);
                    return numberDouble;
                })
                .ToList();

            var sheetsNumbers = DrawingUtils.SheetNumbers(orderedDrawings);
            var sheetsNames = DrawingUtils.GetDrawingsTitles(orderedDrawings);

            CreateTable(sheetsNumbers, sheetsNames.ToArray(), headerTitle);
        }


        /// <summary>
        ///     Create table of drawings names.
        /// </summary>
        /// <param name="sheetsNumbers">Drawings numbers array</param>
        /// <param name="sheetsNames">Drawings names array</param>
        /// <param name="headerTitle">Table title</param>
        public void CreateTable(double[] sheetsNumbers, string[] sheetsNames,
            string headerTitle = "Ведомость рабочих чертежей основного комплекта")
        {
            //======Table settings==========

            //Header
            var headerHeight = 15;
            string[] headerText = {"Лист", "Наименование", "Примечание"};

            //Row
            var rows = sheetsNames.Length;
            var rowHeight = 8;

            //Columns width
            double[] columnWidth = {15, 140, 30};
            var tableWidth = columnWidth.Sum();

            //=====Text settings===========
            var textHeight = 2.5;
            var textFont = "Arial Narrow";


            //=====Insert table=======
            try
            {
                var drawingHandler = new TSD.DrawingHandler();
                if (drawingHandler.GetConnectionStatus())
                {
                    var picker = drawingHandler.GetPicker();
                    Point startPoint = null;
                    TSD.ViewBase viewBase = null;

                    picker.PickPoint("Укажите точку вставки таблицы", out startPoint, out viewBase);

                    DrawTableLines(viewBase, startPoint, tableWidth, rows, rowHeight, headerHeight, columnWidth);

                    DrawTableText(sheetsNumbers, sheetsNames, textHeight, textFont, viewBase, startPoint, tableWidth,
                        rowHeight, headerTitle, headerText, columnWidth, headerHeight);

                    drawingHandler.GetActiveDrawing().CommitChanges();
                }
            }
            catch (Exception exp)
            {
                // ignored
            }
        }

        private void DrawTableLines(TSD.ViewBase viewBase, Point startPoint, double tableWidth, int rows, int rowHeight,
            int headerHeight, double[] columnWidth)
        {
            //=========Drawing table=========
            var lineAtt = new TSD.Line.LineAttributes();
            var lineTypeAtt = new TSD.LineTypeAttributes(TSD.LineTypes.SolidLine, TSD.DrawingColors.Black);
            lineAtt.Line = lineTypeAtt;

            //Table Header
            var headerLine = new TSD.Line(viewBase, startPoint,
                new Point(startPoint.X + tableWidth, startPoint.Y, startPoint.Z), lineAtt);
            headerLine.Insert();

            //Horizontal lines
            for (var i = 0; i < rows + 1; i++)
            {
                var upperLine = new TSD.Line(viewBase,
                    new Point(startPoint.X, startPoint.Y - (i * rowHeight + headerHeight), startPoint.Z),
                    new Point(startPoint.X + tableWidth, startPoint.Y - (i * rowHeight + headerHeight), startPoint.Z),
                    lineAtt);
                upperLine.Insert();
            }

            //Vertical lines
            var firstVerticalLine = new TSD.Line(viewBase, startPoint,
                new Point(startPoint.X, startPoint.Y - (rows * rowHeight + headerHeight), startPoint.Z), lineAtt);
            firstVerticalLine.Insert();

            double currentWidth = 0;
            for (var i = 0; i < columnWidth.Length; i++)
            {
                currentWidth += columnWidth[i];

                var verticalLine = new TSD.Line(viewBase,
                    new Point(startPoint.X + currentWidth, startPoint.Y, startPoint.Z),
                    new Point(startPoint.X + currentWidth, startPoint.Y - (rows * rowHeight + headerHeight),
                        startPoint.Z),
                    lineAtt);
                verticalLine.Insert();
            }
        }

        private void DrawTableText(double[] sheetNumbers, string[] sheetNames, double textHeight, string textFont,
            TSD.ViewBase viewBase, Point startPoint, double tableWidth, int rowHeight, string headerTitle,
            string[] headerText,
            double[] columnWidth, int headerHeight)
        {
            //=========Drawing text=========
            var textAtt = new TSD.Text.TextAttributes();
            textAtt.Alignment = TSD.TextAlignment.Left;
            textAtt.Font = new TSD.FontAttributes(TSD.DrawingColors.Black, textHeight, textFont, false, false);

            //Header text
            var titleAtt = new TSD.Text.TextAttributes();
            titleAtt.Alignment = TSD.TextAlignment.Left;
            titleAtt.Font = new TSD.FontAttributes(TSD.DrawingColors.Black, textHeight + 2, textFont, false, false);
            var titleWord = new TSD.Text(viewBase,
                new Point(startPoint.X + tableWidth / 2, startPoint.Y + rowHeight, startPoint.Z), headerTitle,
                titleAtt);
            ;
            titleWord.Insert();

            TSD.Text headerWord = null;
            double insertXcoord = 0;

            for (var i = 0; i < headerText.Length; i++)
            {
                if (i > 0) insertXcoord += columnWidth[i - 1];

                headerWord = new TSD.Text(viewBase,
                    new Point(startPoint.X + insertXcoord + columnWidth[i] / 2, startPoint.Y - headerHeight / 2,
                        startPoint.Z), headerText[i], textAtt);
                headerWord.Insert();
            }

            //Column number
            TSD.Text sheetNumber = null;

            for (var i = 0; i < sheetNumbers.Length; i++)
            {
                sheetNumber = new TSD.Text(viewBase,
                    new Point(startPoint.X, startPoint.Y - i * rowHeight - headerHeight, startPoint.Z),
                    sheetNumbers[i].ToString(), textAtt);
                sheetNumber.Insert();

                sheetNumber.MoveObjectRelative(new Vector(new Point(columnWidth[0] / 2, -rowHeight / 2,
                    0))); //TextMoveVector(sheetNumber, rowHeight));
                sheetNumber.Modify();
            }

            // SheetNameColumn
            TSD.Text sheetText = null;

            for (var i = 0; i < sheetNames.Length; i++)
                if (!sheetNames[i].Equals(""))
                {
                    sheetText = new TSD.Text(viewBase,
                        new Point(startPoint.X + columnWidth[0], startPoint.Y - i * rowHeight - headerHeight,
                            startPoint.Z),
                        sheetNames[i], textAtt);
                    sheetText.Insert();

                    sheetText.MoveObjectRelative(TextMoveVector(sheetText, rowHeight));
                    sheetText.Modify();
                }
        }

        private Vector TextMoveVector(TSD.Text text, double rowHeight)
        {
            var boundLowerLeft = text.GetObjectAlignedBoundingBox().LowerLeft;
            var textIstPoint = text.InsertionPoint;

            return new Vector(new Point(textIstPoint.X - boundLowerLeft.X, -rowHeight / 2, 0));
        }
    }
}