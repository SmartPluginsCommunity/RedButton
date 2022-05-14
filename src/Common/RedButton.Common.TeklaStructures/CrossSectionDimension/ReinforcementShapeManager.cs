using System;
using System.Collections.Generic;
using System.Linq;
using Tekla.Structures.Drawing;
using Tekla.Structures.Model;
using TSD = Tekla.Structures.Drawing;
using TSM = Tekla.Structures.Model;
using TSG = Tekla.Structures.Geometry3d;


namespace RedButton.Common.TeklaStructures.CrossSectionDimension
{
    internal class ReinforcementShapeManager
    {
        #region Properties
        private TSM.Model _model;
        private DrawingHandler _drawinghandler;
        private ViewBase _drawingview;

        public List<TSG.Point> RebarNormalPoints { get; private set; }
 
        
        #endregion Properties

        #region Constructors

        public ReinforcementShapeManager(ViewBase view)
        {
            _model = new Model();
            _drawinghandler = new DrawingHandler();
            _drawingview = view;
        }

        #endregion Constructors

        #region Inteface

        #endregion

        #region Methods
        public List<TSG.PolyLine> GetRebarsCurves(TSD.Part drawingPart)
        {
            List<TSG.PolyLine> rebarShapePolylineList = new List<TSG.PolyLine>();
            TSM.Part modelPart = _model.SelectModelObject(drawingPart.ModelIdentifier) as TSM.Part;
            var view = (TSD.View)_drawingview;
            bool flag = (modelPart != null);
            var reinforcementEnumerator = view.GetAllObjects(typeof(ReinforcementBase));
            if (flag)
            {
                while (reinforcementEnumerator.MoveNext())
                {
                    var temp = reinforcementEnumerator.Current as TSD.ReinforcementBase;
                    if (temp != null)
                    {
                        var modelReinfocement = _model.SelectModelObject(temp.ModelIdentifier) as Reinforcement;
                        if ((modelReinfocement != null) && (modelReinfocement.Father.Equals(modelPart)))
                        {
                            var rebarGeometry = modelReinfocement.GetRebarGeometries(Reinforcement.RebarGeometryOptionEnum.NONE);
                            for (int i = 0; i < rebarGeometry.Count; i++)
                            {
                                var rebarShape = rebarGeometry[i] as RebarGeometry;
                                if (rebarShape != null) 
                                {
                                    rebarShapePolylineList.Add(rebarShape.Shape);
                                }
                            }
                        }
                    }
                }
            }

            return rebarShapePolylineList;
        }
        public void GetIntersectedPoints(List<TSG.PolyLine> incomeList, double viewDepth)
        {
  
            foreach (var item in incomeList)
            {
                if (item.Points.Count < 3)
                {
                    var startPoint = item.Points[0] as TSG.Point;
                    var endPoint = item.Points[1] as TSG.Point;
                    if (IsPlaneIntersectedByLine(startPoint, endPoint))
                    {
                        RebarNormalPoints.Add(new TSG.Point(startPoint.X, startPoint.Y, 0));
                    }
                }
            }
        }
        public static bool IsPlaneIntersectedByLine(TSG.Point startPoint, TSG.Point endPoint)
        {
            bool flag1 = (endPoint.X - startPoint.X < 0.001) && (endPoint.Y - startPoint.Y < 0.001);
            bool flag2 = (endPoint.Z * startPoint.Z < 1);
            return (flag1 && flag2);
        }
        #endregion Methods
    }
}

