using System;
using System.Linq;
using System.Collections.Generic;
using Tekla.Structures;
using Tekla.Structures.Geometry3d;
using Tekla.Structures.Drawing;
using TSD = Tekla.Structures.Drawing;
using UI = Tekla.Structures.Drawing.UI;


namespace RedButton.Common.TeklaStructures.DrawingTable
{
    /// <summary>
    /// Description of DrawTable.
    /// </summary>
    public class DrawTable
    {
        public DrawTable()
        {
        }

        public string GetCurrentDrawingObjectName()
        {
            string afry_o_name_1 = "";
            string afry_o_name_2 = "";
            string afry_o_name_3 = "";

            string currentDrawingName = "";

            DrawingHandler drawHand = new DrawingHandler();

            if (drawHand.GetConnectionStatus())
            {

                Drawing drawing = drawHand.GetActiveDrawing();
                drawing.GetUserProperty("AFRY_O_NAME_1", ref afry_o_name_1);
                drawing.GetUserProperty("AFRY_O_NAME_2", ref afry_o_name_2);
                drawing.GetUserProperty("AFRY_O_NAME_3", ref afry_o_name_3);
                currentDrawingName = afry_o_name_1 + afry_o_name_2 + afry_o_name_3;
            }
            return currentDrawingName;
        }

        public string GetDrawingObjectName(Drawing drawing)
        {
            string afry_o_name_1 = "";
            string afry_o_name_2 = "";
            string afry_o_name_3 = "";

            string currentDrawingName = "";

            DrawingHandler drawHand = new DrawingHandler();

            if (drawHand.GetConnectionStatus())
            {
                drawing.GetUserProperty("AFRY_O_NAME_1", ref afry_o_name_1);
                drawing.GetUserProperty("AFRY_O_NAME_2", ref afry_o_name_2);
                drawing.GetUserProperty("AFRY_O_NAME_3", ref afry_o_name_3);
                currentDrawingName = afry_o_name_1 + afry_o_name_2 + afry_o_name_3;
            }
            return currentDrawingName;
        }



        public List<Drawing> GetDrawings()
        {
            List<Drawing> drawings = new List<Drawing>();

            DrawingHandler drawHand = new DrawingHandler();

            if (drawHand.GetConnectionStatus())
            {

                DrawingEnumerator drawEnum = drawHand.GetDrawings();

                foreach (Drawing drawing in drawEnum)
                {
                    drawings.Add(drawing);

                }
            }
            return drawings;
        }


        public List<Drawing> GetSelectedDrawings()
        {
            List<Drawing> drawings = new List<Drawing>();

            DrawingHandler drawHand = new DrawingHandler();

            if (drawHand.GetConnectionStatus())
            {
                //DrawingEnumerator drawEnum = drawHand.GetDrawingSelector().GetSelected();
                DrawingEnumerator drawEnum = drawHand.GetDrawings();

                //drawings.AddRange(drawEnum.GetEnumerator);
                foreach (Drawing drawing in drawEnum)
                {
                    drawings.Add(drawing);

                    //string msg = string.Empty;
                    //drawing.GetUserProperty("Default" , ref msg);
                    //	Console.WriteLine("======"+drawings.Count);
                    //	Console.Read();
                }
            }
            return drawings;
        }



        public string[] SheetNames(List<Drawing> drawings)
        {
            Drawing[] drawingsArray = drawings.ToArray();

            string[] sheetNames = new string[drawingsArray.Length];

            string afry_d_name_1 = "";
            string afry_d_name_2 = "";
            string afry_d_name_3 = "";


            for (int i = 0; i < drawingsArray.Length; i++)
            {

                afry_d_name_1 = "";
                afry_d_name_2 = "";
                afry_d_name_3 = "";

                drawingsArray[i].GetUserProperty("AFRY_D_NAME_1", ref afry_d_name_1);
                drawingsArray[i].GetUserProperty("AFRY_D_NAME_2", ref afry_d_name_2);
                drawingsArray[i].GetUserProperty("AFRY_D_NAME_3", ref afry_d_name_3);
                sheetNames[i] = afry_d_name_1 + afry_d_name_2 + afry_d_name_3;    //drawingsArray[i].Title1 + drawingsArray[i].Title2 + drawingsArray[i].Title3;
            }


            //			for (int i = 0; i < drawings.Count; i++) {
            //				sheetNames[i] = drawingsArray[i].Title1 + drawingsArray[i].Title2 + drawingsArray[i].Title3;
            //			}

            return sheetNames;
        }



        public double[] SheetNumbers(List<Drawing> drawings)
        {
            Drawing[] drawingsArray = drawings.ToArray();
            double[] sheetNumbers = new double[drawingsArray.Length];
            string afry_L = null;

            for (int i = 0; i < drawings.Count; i++)
            {
                drawingsArray[i].GetUserProperty("AFRY_L", ref afry_L);
                sheetNumbers[i] = 0;
                sheetNumbers[i] = Double.Parse(afry_L);    //drawingsArray[i].Title1 + drawingsArray[i].Title2 + drawingsArray[i].Title3;
            }

            return sheetNumbers;
        }


        public void CreateTable(double[] sheetNumbers, string[] sheetNames)
        {
            //======Table settings==========
            string headerTitle = "Ведомость рабочих чертежей основного комплекта";

            //Header
            double headerHeight = 15;
            string[] headerText = { "Лист", "Наименование", "Примечание" };

            //Row
            int rows = sheetNames.Length;
            double rowHeight = 8;

            //Columns width
            double[] columnWidth = { 15, 140, 30 };
            double tableWidth = columnWidth.Sum();
            //double tableWidth = columnWidth.Aggregate((x, y) => x + y);     

            //=====Text settings===========
            double textHeight = 2.5;
            string textFont = "Arial Narrow";



            //=====Insert table=======
            try
            {
                DrawingHandler drawingHandler = new DrawingHandler();
                if (drawingHandler.GetConnectionStatus())
                {
                    UI.Picker picker = drawingHandler.GetPicker();
                    Point startPoint = null;
                    ViewBase viewBase = null;


                    picker.PickPoint("Укажите точку вставки таблицы", out startPoint, out viewBase);

                    //=========Drawing table=========

                    TSD.Line.LineAttributes lineAtt = new TSD.Line.LineAttributes();
                    LineTypeAttributes lineTypeAtt = new LineTypeAttributes(LineTypes.SolidLine, DrawingColors.Black);
                    lineAtt.Line = lineTypeAtt;

                    //Table Header
                    TSD.Line headerLine = new TSD.Line(viewBase, startPoint,
                                              new Point(startPoint.X + tableWidth, startPoint.Y, startPoint.Z), lineAtt);
                    headerLine.Insert();



                    //Horizontal lines
                    for (int i = 0; i < rows + 1; i++)
                    {
                        TSD.Line upperLine = new TSD.Line(viewBase, new Point(startPoint.X, startPoint.Y - (i * rowHeight + headerHeight), startPoint.Z),
                                                 new Point(startPoint.X + tableWidth, startPoint.Y - (i * rowHeight + headerHeight), startPoint.Z), lineAtt);
                        upperLine.Insert();
                    }


                    //Vertical lines
                    TSD.Line firstVerticalLine = new TSD.Line(viewBase, startPoint,
                                                     new Point(startPoint.X, startPoint.Y - (rows * rowHeight + headerHeight), startPoint.Z), lineAtt);
                    firstVerticalLine.Insert();

                    double currentWidth = 0;
                    for (int i = 0; i < columnWidth.Length; i++)
                    {
                        currentWidth += columnWidth[i];

                        TSD.Line verticalLine = new TSD.Line(viewBase, new Point(startPoint.X + currentWidth, startPoint.Y, startPoint.Z),
                                                    new Point(startPoint.X + currentWidth, startPoint.Y - (rows * rowHeight + headerHeight), startPoint.Z), lineAtt);
                        verticalLine.Insert();
                    }


                    //=========Drawing text=========

                    Text.TextAttributes textAtt = new Text.TextAttributes();
                    textAtt.Alignment = TextAlignment.Left;
                    textAtt.Font = new FontAttributes(DrawingColors.Black, textHeight, textFont, false, false);
                    //textAtt.PreferredPlacing = PreferredTextPlacingTypes.PointPlacingType();

                    //Header text

                    Text.TextAttributes titleAtt = new Text.TextAttributes();
                    titleAtt.Alignment = TextAlignment.Left;
                    titleAtt.Font = new FontAttributes(DrawingColors.Black, textHeight + 2, textFont, false, false);
                    //textAtt.PreferredPlacing = PreferredTextPlacingTypes.PointPlacingType();
                    TSD.Text titleWord = new TSD.Text(viewBase, new Point(startPoint.X + (tableWidth / 2), startPoint.Y + rowHeight, startPoint.Z), headerTitle, titleAtt); ;
                    titleWord.Insert();


                    TSD.Text headerWord = null;
                    double insertXcoord = 0;

                    for (int i = 0; i < headerText.Length; i++)
                    {

                        if (i > 0)
                        {
                            insertXcoord += columnWidth[i - 1];
                        }
                        headerWord = new TSD.Text(viewBase, new Point(startPoint.X + insertXcoord + (columnWidth[i] / 2), startPoint.Y - (headerHeight / 2), startPoint.Z), headerText[i], textAtt);
                        headerWord.Insert();

                        //						headerWord.MoveObjectRelative(new Vector(new Point()));
                        //						headerWord.Modify();

                    }



                    //NumerColumn

                    TSD.Text sheetNumber = null;

                    for (int i = 0; i < sheetNumbers.Length; i++)
                    {
                        sheetNumber = new TSD.Text(viewBase, new Point(startPoint.X, startPoint.Y - (i * rowHeight) - headerHeight, startPoint.Z), sheetNumbers[i].ToString(), textAtt);
                        sheetNumber.Insert();

                        sheetNumber.MoveObjectRelative(new Vector(new Point((columnWidth[0] / 2), (-rowHeight / 2), 0)));            //TextMoveVector(sheetNumber, rowHeight));
                        sheetNumber.Modify();
                    }

                    // SheetNameColumn
                    TSD.Text sheetText = null;

                    //https://developer.tekla.com/tekla-structures/api/14/11372
                    //https://forum.tekla.com/index.php?/topic/16089-align-drawing-objects/?hl=text#entry70453
                    for (int i = 0; i < sheetNames.Length; i++)
                    {
                        if (!sheetNames[i].Equals(""))
                        {
                            sheetText = new TSD.Text(viewBase, new Point(startPoint.X + columnWidth[0], startPoint.Y - (i * rowHeight) - headerHeight, startPoint.Z), sheetNames[i], textAtt);
                            sheetText.Insert();

                            sheetText.MoveObjectRelative(TextMoveVector(sheetText, rowHeight));
                            sheetText.Modify();
                        }
                    }


                    drawingHandler.GetActiveDrawing().CommitChanges();
                }

            }
            catch (Exception exp)
            {


            }
        }


        private Vector TextMoveVector(TSD.Text text, double rowHeight)
        {
            Point boundLowerLeft = text.GetObjectAlignedBoundingBox().LowerLeft;
            Point textIstPoint = text.InsertionPoint;

            return new Vector(new Point(textIstPoint.X - boundLowerLeft.X, -rowHeight / 2, 0));//boundLowerLeft.Y - textIstPoint.Y
        }

    }
}