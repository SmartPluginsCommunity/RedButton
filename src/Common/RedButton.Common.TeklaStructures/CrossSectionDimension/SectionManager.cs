using System.Collections;
using System.Collections.Generic;
using Tekla.Structures.Drawing;
using Tekla.Structures.Geometry3d;
using Tekla.Structures.Model;
using TSD = Tekla.Structures.Drawing;
using TSM = Tekla.Structures.Model;
using System.Linq;

namespace RedButton.Common.TeklaStructures.CrossSectionDimension
{
    class DrawingSectionManager
    {
        #region Properties
        private TSM.Model _model;
        private DrawingHandler _drawinghandler;
        private ViewBase _pickedview;
        private DrawingObject _drawingObject;
        private TSM.Part _pickedpart;
        private TSD.View _view;
        // Интерфейс с правилами для получения точек 
        private IDrawingSectionRule _rule;
        public List<List<Point>> Points {get; private set;}
        #endregion Properties

        #region Constructors

        public DrawingSectionManager ()
        {
            _model = new Model();
            _drawinghandler = new DrawingHandler();
        }

        #endregion Constructors
        
        #region Inteface
        public interface IDrawingSectionRule
        {
            //TODO Create the interface
        }
        #endregion

        #region Methods
        public void Init(IDrawingSectionRule rule)
        {
            _rule = rule;
            var picker = _drawinghandler.GetPicker();
            picker.PickObject("Укажите деталь на виде", out _drawingObject, out _pickedview);
            _view = (TSD.View)_pickedview;
            TSD.Part drawingPickedPart = _drawingObject as TSD.Part;
            var modelPart = _model.SelectModelObject(drawingPickedPart.ModelIdentifier) as TSM.Part;
            if (modelPart !=null)
                GetIntersection();
        }
    
        private void SetViewCoordinateSystem ()
        {
            _model.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TransformationPlane());
            _model.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TransformationPlane(_view.DisplayCoordinateSystem));
        }

        private void SetModelCoordinateSystem ()
        {
            _model.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TransformationPlane());
            _model.CommitChanges();
        }

        //TODO Разбить на методы и реализовать через LINQ
        private void GetIntersection()
        {
            // add rules
            var solid = _pickedpart.GetSolid(Solid.SolidCreationTypeEnum.HIGH_ACCURACY);
            SetViewCoordinateSystem();
            var viewOriginPoint = _view.DisplayCoordinateSystem.Origin;
            List<List<Point>> sectionCotoursList = new List<List<Point>>();
            var enumerator = solid.IntersectAllFaces(
                viewOriginPoint,
                new Point(viewOriginPoint.X + 10, viewOriginPoint.Y, viewOriginPoint.Z),
                new Point(viewOriginPoint.X, viewOriginPoint.Y + 10, viewOriginPoint.Z)
                );
            while (enumerator.MoveNext())
            {
                var arrayList = enumerator.Current as ArrayList;
                for (int i = 0; i < arrayList.Count; i++)
                {
                    var temp = arrayList[i] as ArrayList;
                    sectionCotoursList.Add(temp.Cast<Point>().ToList());
                }
            }

            Points = sectionCotoursList;
            SetModelCoordinateSystem();
        }
        #endregion Methods
    }
}
