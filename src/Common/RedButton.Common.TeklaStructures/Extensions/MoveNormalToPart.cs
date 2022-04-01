using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tekla.Structures.Model;
using Tekla.Structures.Model.UI;
using Tekla.Structures.Geometry3d;

namespace RedButton.Common.TeklaStructures.Extensions
{
    /// <summary>
    /// Класс предназначен для методов, перемещающих элементы в модели перпендикулярно заданным.
    /// /// hhh
    /// </summary>
    public static class MoveNormalToPart
    {
        /// <summary>
        /// Метод предназначен для перемещения элемента (его точки начала) перпендикулярно к выбранному элементу.
        /// stat - несмещаемый элемент, move - смещаемый
        /// </summary>
        /// <param name="stat"></param>
        /// <param name="move"></param>
        public static void MoveNormal(Beam stat, Beam move)
        {
            var model = new Model();

            if (stat == null || move == null || stat == move)
            {
                Tekla.Structures.Model.Operations.Operation.DisplayPrompt("Выберите объекты.");
                return;
            }

            var currentTP = model.GetWorkPlaneHandler().GetCurrentTransformationPlane();
            var csStat = stat.GetCoordinateSystem();
            var tpStat = new TransformationPlane(csStat);
            model.GetWorkPlaneHandler().SetCurrentTransformationPlane(tpStat);
            // Без применения этих методов координаты в локальной системе элемента не обновляются.
            stat.Select();
            move.Select();

            if (move.StartPoint.X > stat.GetSolid().MaximumPoint.X || move.StartPoint.X < stat.GetSolid().MinimumPoint.X)
            {
                Tekla.Structures.Model.Operations.Operation.DisplayPrompt("Объект за пределами стационарного.");
                return;
            }
            else
            {
                Vector vec1 = new Vector(0, Math.Round(-move.StartPoint.Y + stat.StartPoint.Y, 1), Math.Round(-move.StartPoint.Z, 1));
                Tekla.Structures.Model.Operations.Operation.MoveObject(move, vec1);
            }

            model.GetWorkPlaneHandler().SetCurrentTransformationPlane(currentTP);
            model.CommitChanges();
        }
    }
}
