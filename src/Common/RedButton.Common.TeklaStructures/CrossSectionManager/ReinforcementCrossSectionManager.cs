using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using Tekla.Structures.Drawing;
using Tekla.Structures.Geometry3d;
using Tekla.Structures.Model;
using TSD = Tekla.Structures.Drawing;
using TSM = Tekla.Structures.Model;

namespace RedButton.Common.TeklaStructures.CrossSectionManager
{
    class ReinforcementCrossSectionManager
    {
        #region Properties
        private TSM.Model _model;
        private DrawingHandler _drawinghandler;
        private ViewBase _pickedview;
        private DrawingObject _drawingObject;
        private TSM.Part _pickedpart;
        //public List<List<Point>> Points { get; private set; }
        #endregion Properties

        #region Constructors

        public ReinforcementCrossSectionManager()
        {
            _model = new Model();
            _drawinghandler = new DrawingHandler();
        }

        #endregion Constructors

        #region Methods
        public void Init()
        {
            var picker = _drawinghandler.GetPicker();
            picker.PickObject("Укажите деталь на виде", out _drawingObject, out _pickedview);
            TSD.Part drawingPickedPart = _drawingObject as TSD.Part;
            var modelPart = _model.SelectModelObject(drawingPickedPart.ModelIdentifier) as TSM.Part;
            TSM.ModelObjectEnumerator barEnumerator = modelPart.GetReinforcements();
            while (barEnumerator.MoveNext())
            {
                var reinforcement = barEnumerator.Current as TSM.Reinforcement ;
                if (reinforcement is RebarGroup)
                {
                    var rebarGeoArray = reinforcement.GetRebarGeometries(Reinforcement.RebarGeometryOptionEnum.NONE);

                    {
                           
                    }
                }
                 

            }
            if (modelPart != null)
                GetIntersection();
            
        }
        void Bars()
        {
            
        }

        //TODO Разбить на методы и реализовать через LINQ
        private void GetIntersection()
        {
            // add rules
            var solid = _pickedpart.GetSolid(Solid.SolidCreationTypeEnum.NORMAL);
            var viewOriginPoint = _pickedview.Origin;
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

            //Points = sectionCotoursList;
        }
        #endregion Methods
    }
}
