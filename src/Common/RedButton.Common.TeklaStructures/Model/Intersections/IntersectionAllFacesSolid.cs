using RedButton.Common.TeklaStructures.Model.Geometry;
using System.Collections.Generic;
using System.Linq;
using Tekla.Structures.Geometry3d;

namespace RedButton.Common.TeklaStructures.Model.Intersections
{
    public struct IntersectionAllFacesSolid
    {
        /// <summary>
        /// A outer polygon
        /// </summary>
        public Polygon OuterContour { get; }

        /// <summary>
        /// Inner polygons
        /// </summary>
        public IEnumerable<Polygon> InnerContours { get; }

        /// <summary>
        /// .ctor
        /// </summary>
        /// <param name="outerContour"></param>
        /// <param name="innerContours"></param>
        public IntersectionAllFacesSolid(IEnumerable<Point> outerContour, IEnumerable<IEnumerable<Point>> innerContours)
        {
            OuterContour = new Polygon(outerContour);
            InnerContours = innerContours?.Select(points => new Polygon(points));
        }
    }
}
