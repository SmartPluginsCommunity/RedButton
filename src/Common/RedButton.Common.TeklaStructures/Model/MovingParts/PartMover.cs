using System;
using Tekla.Structures.Model;
using Tekla.Structures.Model.UI;
using Tekla.Structures.Geometry3d;

namespace RedButton.Common.TeklaStructures.Model.MovingParts
{
    /// <summary>
    /// Class with methods to move parts.
    /// </summary>
    public class PartMover
    {
        /// <summary>
        /// Move a beam normal to selected beam.
        /// </summary>
        /// <param name="baseObject"></param>
        /// <param name="movingObject"></param>
        public void MoveBeamNormalToBeam(Beam baseObject, Beam movingObject)
        {
            var model = new Model();

            if (baseObject == null || movingObject == null || baseObject == movingObject)
            {
                Tekla.Structures.Model.Operations.Operation.DisplayPrompt("Выберите объекты.");
                return;
            }

            var currentTP = model.GetWorkPlaneHandler().GetCurrentTransformationPlane();
            var csBaseObject = baseObject.GetCoordinateSystem();
            var tpBaseObject = new TransformationPlane(csBaseObject);
            model.GetWorkPlaneHandler().SetCurrentTransformationPlane(tpBaseObject);
            // Method .Select() refresh beam's coordinate sistem after a transfomation plane change.
            baseObject.Select();
            movingObject.Select();

            if (movingObject.StartPoint.X > baseObject.GetSolid().MaximumPoint.X || movingObject.StartPoint.X < baseObject.GetSolid().MinimumPoint.X)
            {
                Tekla.Structures.Model.Operations.Operation.DisplayPrompt("Объект за пределами стационарного.");
                return;
            }
            else
            {
                var MoveVector = new Vector(0, Math.Round(-movingObject.StartPoint.Y + baseObject.StartPoint.Y, 1), Math.Round(-movingObject.StartPoint.Z, 1));
                Tekla.Structures.Model.Operations.Operation.MoveObject(movingObject, vec1);
            }

            model.GetWorkPlaneHandler().SetCurrentTransformationPlane(currentTP);
            model.CommitChanges();
        }
    }
}
