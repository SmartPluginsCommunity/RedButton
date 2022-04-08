using RedButton.Common.Core.CollectionExtensions;
using RedButton.Common.TeklaStructures.Model;
using RedButton.Common.TeklaStructures.Model.Intersections;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Tekla.Structures.Geometry3d;
using Tekla.Structures.Model;

namespace RedButton.Common.TeklaStructures.Extensions
{
    public static class SolidExtension
    {
        /// <summary>
        /// Returns all polygons for an array list of lists of plane - solid intersection points from all intersecting faces.
        /// </summary>
        /// <param name="solid">Solid</param>
        /// <param name="p1">Point 1</param>
        /// <param name="p2">Point 2</param>
        /// <param name="p3">Point 3</param>
        /// <returns></returns>
        public static IntersectionAllFacesSolid GetAllContours(this Solid solid, Point p1, Point p2, Point p3)
        {
            var intersections = GetIntersections(solid, p1, p2, p3);
            return new IntersectionAllFacesSolid(intersections?.FirstOrDefault(), intersections?.Skip(1));
        }

        /// <summary>
        /// Returns all polygons for an array list of lists of plane - solid intersection points from all intersecting faces.
        /// </summary>
        /// <param name="solid">Solid</param>
        /// <param name="plane">Plane</param>
        /// <returns></returns>
        public static IntersectionAllFacesSolid GetAllContours(this Solid solid, Plane plane)
        {
            var intersections = GetIntersections(solid, plane.Origin, plane.AxisX, plane.AxisY);
            return new IntersectionAllFacesSolid(intersections?.FirstOrDefault(), intersections?.Skip(1));
        }

        /// <summary>
        /// Get intersections for solid
        /// </summary>
        /// <param name="solid">Solid</param>
        /// <param name="p1">Point 1</param>
        /// <param name="p2">Point 2</param>
        /// <param name="p3">Point 3</param>
        /// <returns></returns>
        private static IEnumerable<IEnumerable<Point>> GetIntersections(Solid solid, Point p1, Point p2, Point p3)
        {
            if(solid == null)
                return null;

            return solid.IntersectAllFaces(p1, p2, p3)
                        .ToIEnumerable<ArrayList>()
                        ?.FirstOrDefault()
                        .OfType<ArrayList>()
                        .Select(points => points.OfType<Point>().ToList())
                        .ToList();
        }
    }
}
