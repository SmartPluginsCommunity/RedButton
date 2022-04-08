using System.Collections.Generic;
using Tekla.Structures.Geometry3d;

namespace RedButton.Common.TeklaStructures.Model.Geometry
{
    public struct Polygon
    {
        /// <summary>
        /// Points
        /// </summary>
        public IEnumerable<Point> Points { get;}

        /// <summary>
        /// .ctor
        /// </summary>
        /// <param name="points"></param>
        public Polygon(IEnumerable<Point> points)
        {
            Points = points;
        }
    }
}
