using System;
using Tekla.Structures.Drawing;
using Tekla.Structures.Model;
using  RedButton.Common.Core.CollectionExtensions;

using Tekla.Structures.Geometry3d;
using System.Collections.Generic;
using Part = Tekla.Structures.Drawing.Part;
using System.Linq;
using Line = Tekla.Structures.Geometry3d.Line;
using TSM = Tekla.Structures.Model;
using TSD = Tekla.Structures.Drawing;

namespace RedButton.Common.TeklaStructures.Dim
{
    public class SimpleDimension
    {
        private Beam MyBeam = new Beam();
        private TransformationPlane StartPlane;
        private StraightDimensionSet.StraightDimensionSetAttributes DimAttributes;
        private ViewBase MyViewBase;
        private double ScaleView;
        private static StraightDimensionSet.StraightDimensionSetAttributes _dimAttributes;
        
        public void SimpleSectionDimension()
        {
            new Exception();
            try
            {
                var drawingHandler = new DrawingHandler();

                if (!drawingHandler.GetConnectionStatus()) return;
                var currentDrawing = drawingHandler.GetActiveDrawing();
                if (currentDrawing == null) return;
                var myModel = new TSM.Model();
                var myWorkPlaneHandler = myModel.GetWorkPlaneHandler();
                StartPlane = myWorkPlaneHandler.GetCurrentTransformationPlane();

                //загружаем настройки аттрибутов размеров
                DimAttributes = new StraightDimensionSet.StraightDimensionSetAttributes(null, "MyAttributes");
                SetDimAttributes(DimAttributes);

                //пока не знаю как программно выбрать вид для образмеривания, поэтому выбираю с помощью пикера. Для этого нужно выбрать точку, но она в дальнейшем не нужна 
                var isPart = false;
                while (!isPart)
                {
                    var picker = new DrawingHandler().GetPicker();
                    picker.PickObject("Выберите вид для образмеривания", out var dObj, out MyViewBase);
                    ScaleView = (MyViewBase as View).Attributes.Scale;

                    if (dObj is Part)
                    {
                        isPart = true;
                        var myPart = (Part)dObj;
                        var id = myPart.ModelIdentifier;
                        //выбираем балку в модели, соотвутствующую изображению на чертеже
                        MyBeam.Identifier = id;
                        MyBeam.Select();

                        //получаем текущий вид
                        var currentView = MyViewBase as View;
                        var cs = currentView.ViewCoordinateSystem;
                        var viewPlane = new TransformationPlane(currentView.ViewCoordinateSystem);
                        var vecBeam = new Vector().TwoPointVector(MyBeam.EndPoint, MyBeam.StartPoint).GetNormal();
                        //список полигонов
                        var listPolygons = new List<TSM.Polygon>();

                        //получаем лист полигонов (включающий координаты границ объекта на чертеже)
                        CS.GetProjectedShape.GetShape(id, ref listPolygons);
                        var polygon = listPolygons[0];

                        //получаем объект модели
                        new TSM.Model().SelectModelObject(id);
                        
                        //выбираем группы продольной арматуры
                        var longRg = MyBeam.GetChildren().ToList<TSM.ModelObject>().OfType<RebarGroup>().Where(g =>
                        {
                            var vecRg = new Vector().TwoPointVector(g.EndPoint, g.StartPoint).GetNormal();
                            return !vecRg.Equals(vecBeam);
                        }).ToList();
                        //получаем список точек армирования
                        var points = (from rg in longRg from RebarGeometry geometry in rg.GetRebarGeometries(false) select (Point) geometry.Shape.Points[0]).ToList();
                        points.AddRange(polygon.Points.ToPointList());
                        
                        //находим точки проекций арматуры на оси Х и У
                        var projectedPointsX = (from p in points 
                            select Projection.PointToLine(p, new Line((Point) polygon.Points[0], (Point) polygon.Points[1]))).Distinct().ToList();
                        var projectedPointsY = (from p in points 
                            select Projection.PointToLine(p, new Line((Point) polygon.Points[1], (Point) polygon.Points[2]))).Distinct().ToList();
                        //приводим точки в координатную систему вида чертежа
                        var pointX = projectedPointsX.Select(p => p.TransformToLocal(cs)).ToList();
                        var pointY = projectedPointsY.Select(p => p.TransformToLocal(cs)).ToList();
                        
                        //чертим размеры
                        CreateMultiDim(MyViewBase,40.0,pointX, DimAttributes);
                        CreateMultiDim(MyViewBase,40.0,pointY, DimAttributes);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.ToString());
            }
        }
        
        public void SetDimAttributes(StraightDimensionSet.StraightDimensionSetAttributes dimAttributes)
        {
            dimAttributes.DimensionType = DimensionSetBaseAttributes.DimensionTypes.Relative; //тип размеров только такой
            dimAttributes.Exaggeration.ExaggerationEnabled = false;
            dimAttributes.Format.Unit = DimensionSetBaseAttributes.DimensionValueUnits.Millimeter;
            dimAttributes.Placing.Distance.MaximalDistance = 10;
            dimAttributes.Placing.Distance.MinimalDistance = 1;
            dimAttributes.Placing.Distance.SearchMargin = 5;
            dimAttributes.Placing.Direction.Negative = true;
            dimAttributes.Placing.Placing = DimensionSetBaseAttributes.Placings.Fixed;        //положение размеров фиксировано, почему-то портит мне жизнь
            dimAttributes.Text.Font.Height = 2.40;
            dimAttributes.Text.Font.Italic = false;
            dimAttributes.Text.Font.Name = "GOST 2.304 type A";
            dimAttributes.Text.TextPlacing = DimensionSetBaseAttributes.DimensionTextPlacings.AboveDimensionLine;
            dimAttributes.Text.Frame = DimensionSetBaseAttributes.FrameTypes.None;
            dimAttributes.Arrowhead.Height = 1.5;
            dimAttributes.Arrowhead.Width = 1.5;
            dimAttributes.Arrowhead.ArrowPosition = ArrowheadPositions.Both;
            dimAttributes.Arrowhead.Head = ArrowheadTypes.NoArrow;
            dimAttributes.ShortDimension = DimensionSetBaseAttributes.ShortDimensionTypes.Outside;
            dimAttributes.ExtensionLine = DimensionSetBaseAttributes.ExtensionLineTypes.Yes;
            dimAttributes.LeftLowerTag.Clear();
        }
        
        public void CreateMultiDim(ViewBase myViewBase, double scaleView, List<Point> points, 
            StraightDimensionSet.StraightDimensionSetAttributes dimAttributes)
        {
            //определяем параметр дистанции размера от объекта
            var dist = 10 * scaleView;
            //определяем вектор размера (ортогональный размерной линии)
            var vecWork = new Vector().TwoPointVector(points[0], points[points.Count - 1]);
            var vecDim = -1*vecWork.GetOrthoVector2d();
            var dimSet = CreateDimensionSet(myViewBase, points[0], points[points.Count-1], vecDim);
            for (var i = 0; i < points.Count-1; i += 2)
            {
                var dimTemp = CreateDimensionSet(myViewBase, points[i], points[i+1], vecDim);
                dimSet.AddToDimensionSet(dimTemp);
            }

            dimSet.Attributes = dimAttributes;
            dimSet.Distance = dist;
            dimSet.Modify();
        }
        
    }
}