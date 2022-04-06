using RedButton.Common.Core.CollectionExtensions;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Tekla.Structures.Geometry3d;
using Tekla.Structures.Model;

namespace RedButton.Common.TeklaStructures.Extensions
{
    public static class PartExtension
    {
        /// <summary>
        /// Get a outer contour. It's intersect with plane (three points) and solid
        /// </summary>
        /// <param name="part">Деталь, с которой ищется пересечение.</param>
        /// <param name="solidType">Тип Solid.</param>
        /// <returns></returns>
        public static IEnumerable<Point> GetOuterContour(this Part part, Point p1, Point p2, Point p3, Solid.SolidCreationTypeEnum solidType)
        {
            return GetIntersections(part.GetSolid(solidType), p1, p2, p3)?.FirstOrDefault();
        }

        /// <summary>
        /// Get all contours. They're intersect with plane (three points) and solid. 
        /// A outer contout is first position.
        /// </summary>
        /// <param name="part">Деталь, с которой ищется пересечение.</param>
        /// <param name="solidType">Тип Solid.</param>
        /// <returns></returns>
        public static IEnumerable<IEnumerable<Point>> GetAllContours(this Part part, Point p1, Point p2, Point p3, Solid.SolidCreationTypeEnum solidType)
        {
            return GetIntersections(part.GetSolid(solidType), p1, p2, p3);
        }

        private static IEnumerable<IEnumerable<Point>> GetIntersections(Solid solid, Point p1, Point p2, Point p3)
        {
            if(solid == null)
                return null;

            return solid.IntersectAllFaces(p1, p2, p3).ToIEnumerable<ArrayList>().Select(points => points.OfType<Point>().ToList()).ToList();
        }
    }
}
