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
        private List<Tekla.Structures.Geometry3d.Point> dynamicPoints;
        private List<Tekla.Structures.Model.Polygon> dynamicPolygons;
        private List<ModelObject> dynamicObjects;
        private List<Matrix> listOfMatrices;
        private List<TransformationPlane> listOfOriginalPlanes;

        public SetPlane(Tekla.Structures.Model.Model actualModel)
        {
            this.dynamicPoints = new List<Tekla.Structures.Geometry3d.Point>();
            this.dynamicPolygons = new List<Tekla.Structures.Model.Polygon>();
            this.dynamicObjects = new List<ModelObject>();
            this.listOfMatrices = new List<Matrix>();
            this.listOfOriginalPlanes = new List<TransformationPlane>();
            this._model = actualModel;
        }

        public void Begin(CoordinateSystem newSystem) => this.Begin(newSystem.Origin, newSystem.AxisX, newSystem.AxisY);

        public void Begin(Tekla.Structures.Geometry3d.Point newOrigin, Vector newVectorX, Vector newVectorY)
        {
            try
            {
                newVectorY = Geo.GetNormalVectorInPlane(newVectorX, newVectorY);
                CoordinateSystem coordinateSystem1 = new CoordinateSystem(newOrigin, newVectorX, newVectorY);
                TransformationPlane TransformationPlane = new TransformationPlane(coordinateSystem1);
                this.listOfOriginalPlanes.Add(this._model.GetWorkPlaneHandler().GetCurrentTransformationPlane());
                Matrix coordinateSystem2 = MatrixFactory.ToCoordinateSystem(coordinateSystem1);
                this.listOfMatrices.Add(coordinateSystem2);
                this.TransformAll(coordinateSystem2);
                this._model.GetWorkPlaneHandler().SetCurrentTransformationPlane(TransformationPlane);
                this.TransformModelObjects();
            }
            catch (Exception)
            {
            }
        }

        public void TransformAll(Matrix transformationMatrix)
        {
            if (this.dynamicPoints != null)
                this.TransformPoints(transformationMatrix);
            if (this.dynamicPolygons == null)
                return;
            this.TransformPolygons(transformationMatrix);
        }

        public void TransformPoints(Matrix transformationMatrix)
        {
            for (int index = 0; index < this.dynamicPoints.Count; ++index)
            {
                Tekla.Structures.Geometry3d.Point orginalPoint = transformationMatrix.Transform(this.dynamicPoints[index]);
                Geo.CopyPointPosition(this.dynamicPoints[index], orginalPoint);
            }
        }

        public void TransformPoints(Matrix transformationMatrix, ArrayList pointsToMoveList)
        {
            try
            {
                for (int index = 0; index < pointsToMoveList.Count; ++index)
                {
                    Tekla.Structures.Geometry3d.Point orginalPoint = transformationMatrix.Transform((Tekla.Structures.Geometry3d.Point)pointsToMoveList[index]);
                    Geo.CopyPointPosition((Tekla.Structures.Geometry3d.Point)pointsToMoveList[index], orginalPoint);
                }
            }
            catch (Exception)
            {
            }
        }

        public void TransformPolygons(Matrix transformationMatrix)
        {
            for (int index = 0; index < this.dynamicPolygons.Count; ++index)
                this.TransformPoints(transformationMatrix, this.dynamicPolygons[index].Points);
        }

        public void TransformModelObjects()
        {
            if (this.dynamicObjects == null)
                return;
            for (int index = 0; index < this.dynamicObjects.Count; ++index)
                this.dynamicObjects[index].Select();
        }

        public void End()
        {
            int index1 = this.listOfMatrices.Count - 1;
            int index2 = this.listOfOriginalPlanes.Count - 1;
            if (index1 <= -1 || index2 <= -1)
                return;
            Matrix listOfMatrix = this.listOfMatrices[index1];
            listOfMatrix.Transpose();
            this.TransformAll(listOfMatrix);
            TransformationPlane listOfOriginalPlane = this.listOfOriginalPlanes[index2];
            this._model.GetWorkPlaneHandler().SetCurrentTransformationPlane(listOfOriginalPlane);
            this.TransformModelObjects();
            this.listOfMatrices.RemoveAt(index1);
            this.listOfOriginalPlanes.RemoveAt(index2);
        }

        public Tekla.Structures.Geometry3d.Point Point(double X, double Y, double Z)
        {
            Tekla.Structures.Geometry3d.Point point = new Tekla.Structures.Geometry3d.Point(X, Y, Z);
            this.dynamicPoints.Add(point);
            return point;
        }

        public Tekla.Structures.Model.Polygon Polygon()
        {
            Tekla.Structures.Model.Polygon polygon = new Tekla.Structures.Model.Polygon();
            this.dynamicPolygons.Add(polygon);
            return polygon;
        }

        public void AddPoints(params Tekla.Structures.Geometry3d.Point[] dynamicPointsToAdd) => this.dynamicPoints.AddRange((IEnumerable<Tekla.Structures.Geometry3d.Point>)dynamicPointsToAdd);

        public void AddPolygons(params Tekla.Structures.Model.Polygon[] dynamicPolygonsToAdd) => this.dynamicPolygons.AddRange((IEnumerable<Tekla.Structures.Model.Polygon>)dynamicPolygonsToAdd);

        public void AddModelObjects(params ModelObject[] dynamicObjectsToAdd) => this.dynamicObjects.AddRange((IEnumerable<ModelObject>)dynamicObjectsToAdd);

        public void AddArrayList(ArrayList list)
        {
            if (list == null)
                return;
            for (int index = 0; index < list.Count; ++index)
            {
                if (list[index] is ArrayList)
                    this.AddArrayList(list[index] as ArrayList);
                else if ((object)(list[index] as Tekla.Structures.Geometry3d.Point) != null)
                {
                    Tekla.Structures.Geometry3d.Point point = (Tekla.Structures.Geometry3d.Point)list[index];
                    if (point != (Tekla.Structures.Geometry3d.Point)null)
                        this.AddPoints(point);
                }
                else if (list[index] is Tekla.Structures.Model.Polygon)
                {
                    Tekla.Structures.Model.Polygon polygon = (Tekla.Structures.Model.Polygon)list[index];
                    if (polygon != null)
                        this.AddPolygons(polygon);
                }
                else if (list[index] is ModelObject)
                    this.AddModelObjects(list[index] as ModelObject);
            }
        }

        public void RemovePoints(params Tekla.Structures.Geometry3d.Point[] dynamicPointsToRemove)
        {
            for (int index = 0; index < dynamicPointsToRemove.Length; ++index)
                this.dynamicPoints.Remove(dynamicPointsToRemove[index]);
        }

        public void RemovePolygons(params Tekla.Structures.Model.Polygon[] dynamicPolygonsToRemove)
        {
            for (int index = 0; index < dynamicPolygonsToRemove.Length; ++index)
                this.dynamicPolygons.Remove(dynamicPolygonsToRemove[index]);
        }

        public void RemoveModelObjects(params ModelObject[] dynamicObjectsToRemove)
        {
            for (int index = 0; index < dynamicObjectsToRemove.Length; ++index)
                this.dynamicObjects.Remove(dynamicObjectsToRemove[index]);
        }

        public void RemoveAllPoints() => this.dynamicPoints.Clear();

        public void RemoveAllPolygons() => this.dynamicPolygons.Clear();

        public void RemoveAllModelObjects() => this.dynamicObjects.Clear();

        public void RemoveArrayList(ArrayList list)
        {
            if (list == null)
                return;
            for (int index = 0; index < list.Count; ++index)
            {
                if (list[index] is ArrayList)
                    this.RemoveArrayList(list[index] as ArrayList);
                else if ((object)(list[index] as Tekla.Structures.Geometry3d.Point) != null)
                {
                    Tekla.Structures.Geometry3d.Point point = (Tekla.Structures.Geometry3d.Point)list[index];
                    if (point != (Tekla.Structures.Geometry3d.Point)null)
                        this.RemovePoints(point);
                }
                else if (list[index] is Tekla.Structures.Model.Polygon)
                {
                    Tekla.Structures.Model.Polygon polygon = (Tekla.Structures.Model.Polygon)list[index];
                    if (polygon != null)
                        this.RemovePolygons(polygon);
                }
                else if (list[index] is ModelObject)
                    this.RemoveModelObjects(list[index] as ModelObject);
            }
        }
    }
}
