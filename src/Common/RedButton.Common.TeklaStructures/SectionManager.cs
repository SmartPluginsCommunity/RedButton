using System.Collections;
using System.Collections.Generic;
using Tekla.Structures.Drawing;
using Tekla.Structures.Geometry3d;
using Tekla.Structures.Model;
using TSD = Tekla.Structures.Drawing;
using TSM = Tekla.Structures.Model;

namespace RedButton
{
    class DrawingSectionManager
    {
        #region Properties
        private TSM.Model _model;
        private DrawingHandler _drawinghandler;
        private ViewBase _pickedview;
        private DrawingObject _drawingObject;
        private TSM.Part _pickedpart;

        // Интерфейс с правилами для получения точек 
        private IDrawingSectionRule _rule;

        public List<List<Point>> Points {get; private set;}


        #endregion Properties

        #region Constructors
        public SectionManager()
        {
            _model = new Model();
            _drawinghandler = new DrawingHandler();
            _pickedview = (ViewBase)null;
            _drawingObject = (DrawingObject)null;
        }
        #endregion Constructors

        #region Methods
        public void Init(IDrawingSectionRule rule)
        {
            _rule = rule;
            var picker = _drawinghandler.GetPicker();
            picker.PickObject("Укажите деталь", out _drawingObject, out _pickedview);
            TSD.Part drawingPickedPart = _drawingObject as TSD.Part;
            var modelPart = _model.SelectModelObject(drawingPickedPart.ModelIdentifier) as TSM.Part;
            GetIntersection();
        }
    

        //TODO Разбить на методы и реализовать через LINQ
        private void GetIntersection()
        {
            // add rules
            List<List<Point>> result = new List<List<Point>>();
            var solid = _pickedpart.GetSolid();
            var viewOriginPoint = _pickedview.Origin;
            var enumerator = solid.IntersectAllFaces(
                viewOriginPoint,
                new Point(viewOriginPoint.X + 10, viewOriginPoint.Y, viewOriginPoint.Z),
                new Point(viewOriginPoint.X, viewOriginPoint.Y + 10, viewOriginPoint.Z)
                ); 
            int faceIndex = 0;
            while (enumerator.MoveNext())
            {
                ArrayList Points = enumerator.Current as ArrayList;
                IEnumerator LoopsEnum = Points.GetEnumerator();

                int loopIndex = 0;
                while (LoopsEnum.MoveNext())
                {
                    ArrayList LoopPoints = LoopsEnum.Current as ArrayList;
                    if (LoopPoints != null)
                    {
                        result.Add(new List<Point>());
                        IEnumerator LoopPointsEnum = LoopPoints.GetEnumerator();
                        while (LoopPointsEnum.MoveNext())
                        {
                            Point solidPoint = LoopPointsEnum.Current as Point;
                            if (solidPoint != null)
                            {
                                result[loopIndex].Add(solidPoint);
                            }
                        }
                    }
                    loopIndex++;
                }
                faceIndex++;
            }
            Points = result;
        }
        #endregion Methods
    }
}
