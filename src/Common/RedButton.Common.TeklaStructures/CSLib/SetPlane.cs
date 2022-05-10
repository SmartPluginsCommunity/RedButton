using System;
using System.Collections;
using System.Collections.Generic;
using Tekla.Structures.Geometry3d;
using Tekla.Structures.Model;

namespace RedButton.Common.TeklaStructures.CSLib
{
    public class SetPlane
    {
        private Tekla.Structures.Model.Model _model;
        private List<Point> dynamicPoints;
        private List<Polygon> dynamicPolygons;
        private List<ModelObject> dynamicObjects;
        private List<Matrix> listOfMatrices;
        private List<TransformationPlane> listOfOriginalPlanes;

        public SetPlane(Tekla.Structures.Model.Model actualModel)
        {
            dynamicPoints = new List<Point>();
            dynamicPolygons = new List<Polygon>();
            dynamicObjects = new List<ModelObject>();
            listOfMatrices = new List<Matrix>();
            listOfOriginalPlanes = new List<TransformationPlane>();
            _model = actualModel;
        }

        public void Begin(CoordinateSystem newSystem) => Begin(newSystem.Origin, newSystem.AxisX, newSystem.AxisY);

        public void Begin(Point newOrigin, Vector newVectorX, Vector newVectorY)
        {
            try
            {
                newVectorY = Geo.GetNormalVectorInPlane(newVectorX, newVectorY);
                CoordinateSystem coordinateSystem1 = new CoordinateSystem(newOrigin, newVectorX, newVectorY);
                TransformationPlane TransformationPlane = new TransformationPlane(coordinateSystem1);
                listOfOriginalPlanes.Add(_model.GetWorkPlaneHandler().GetCurrentTransformationPlane());
                Matrix coordinateSystem2 = MatrixFactory.ToCoordinateSystem(coordinateSystem1);
                listOfMatrices.Add(coordinateSystem2);
                TransformAll(coordinateSystem2);
                _model.GetWorkPlaneHandler().SetCurrentTransformationPlane(TransformationPlane);
                TransformModelObjects();
            }
            catch (Exception)
            {
            }
        }

        public void TransformAll(Matrix transformationMatrix)
        {
            if (dynamicPoints != null)
                TransformPoints(transformationMatrix);
            if (dynamicPolygons == null)
                return;
            TransformPolygons(transformationMatrix);
        }

        public void TransformPoints(Matrix transformationMatrix)
        {
            for (int index = 0; index < dynamicPoints.Count; ++index)
            {
                Point orginalPoint = transformationMatrix.Transform(dynamicPoints[index]);
                Geo.CopyPointPosition(dynamicPoints[index], orginalPoint);
            }
        }

        public void TransformPoints(Matrix transformationMatrix, ArrayList pointsToMoveList)
        {
            try
            {
                for (int index = 0; index < pointsToMoveList.Count; ++index)
                {
                    Point orginalPoint = transformationMatrix.Transform((Point)pointsToMoveList[index]);
                    Geo.CopyPointPosition((Point)pointsToMoveList[index], orginalPoint);
                }
            }
            catch (Exception)
            {
            }
        }

        public void TransformPolygons(Matrix transformationMatrix)
        {
            for (int index = 0; index < dynamicPolygons.Count; ++index)
                TransformPoints(transformationMatrix, dynamicPolygons[index].Points);
        }

        public void TransformModelObjects()
        {
            if (dynamicObjects == null)
                return;
            for (int index = 0; index < dynamicObjects.Count; ++index)
                dynamicObjects[index].Select();
        }

        public void End()
        {
            int index1 = listOfMatrices.Count - 1;
            int index2 = listOfOriginalPlanes.Count - 1;
            if (index1 <= -1 || index2 <= -1)
                return;
            Matrix listOfMatrix = listOfMatrices[index1];
            listOfMatrix.Transpose();
            TransformAll(listOfMatrix);
            TransformationPlane listOfOriginalPlane = listOfOriginalPlanes[index2];
            _model.GetWorkPlaneHandler().SetCurrentTransformationPlane(listOfOriginalPlane);
            TransformModelObjects();
            listOfMatrices.RemoveAt(index1);
            listOfOriginalPlanes.RemoveAt(index2);
        }

        public Point Point(double X, double Y, double Z)
        {
            Point point = new Point(X, Y, Z);
            dynamicPoints.Add(point);
            return point;
        }

        public Polygon Polygon()
        {
            Polygon polygon = new Polygon();
            dynamicPolygons.Add(polygon);
            return polygon;
        }

        public void AddPoints(params Point[] dynamicPointsToAdd) => dynamicPoints.AddRange((IEnumerable<Point>)dynamicPointsToAdd);

        public void AddPolygons(params Polygon[] dynamicPolygonsToAdd) => dynamicPolygons.AddRange((IEnumerable<Polygon>)dynamicPolygonsToAdd);

        public void AddModelObjects(params ModelObject[] dynamicObjectsToAdd) => dynamicObjects.AddRange((IEnumerable<ModelObject>)dynamicObjectsToAdd);

        public void AddArrayList(ArrayList list)
        {
            if (list == null)
                return;
            for (int index = 0; index < list.Count; ++index)
            {
                if (list[index] is ArrayList)
                    AddArrayList(list[index] as ArrayList);
                else if ((object)(list[index] as Point) != null)
                {
                    Point point = (Point)list[index];
                    if (point != (Point)null)
                        AddPoints(point);
                }
                else if (list[index] is Polygon)
                {
                    Polygon polygon = (Polygon)list[index];
                    if (polygon != null)
                        AddPolygons(polygon);
                }
                else if (list[index] is ModelObject)
                    AddModelObjects(list[index] as ModelObject);
            }
        }

        public void RemovePoints(params Point[] dynamicPointsToRemove)
        {
            for (int index = 0; index < dynamicPointsToRemove.Length; ++index)
                dynamicPoints.Remove(dynamicPointsToRemove[index]);
        }

        public void RemovePolygons(params Polygon[] dynamicPolygonsToRemove)
        {
            for (int index = 0; index < dynamicPolygonsToRemove.Length; ++index)
                dynamicPolygons.Remove(dynamicPolygonsToRemove[index]);
        }

        public void RemoveModelObjects(params ModelObject[] dynamicObjectsToRemove)
        {
            for (int index = 0; index < dynamicObjectsToRemove.Length; ++index)
                dynamicObjects.Remove(dynamicObjectsToRemove[index]);
        }

        public void RemoveAllPoints() => dynamicPoints.Clear();

        public void RemoveAllPolygons() => dynamicPolygons.Clear();

        public void RemoveAllModelObjects() => dynamicObjects.Clear();

        public void RemoveArrayList(ArrayList list)
        {
            if (list == null)
                return;
            for (int index = 0; index < list.Count; ++index)
            {
                if (list[index] is ArrayList)
                    RemoveArrayList(list[index] as ArrayList);
                else if ((object)(list[index] as Point) != null)
                {
                    Point point = (Point)list[index];
                    if (point != (Point)null)
                        RemovePoints(point);
                }
                else if (list[index] is Polygon)
                {
                    Polygon polygon = (Polygon)list[index];
                    if (polygon != null)
                        RemovePolygons(polygon);
                }
                else if (list[index] is ModelObject)
                    RemoveModelObjects(list[index] as ModelObject);
            }
        }
    }
}
