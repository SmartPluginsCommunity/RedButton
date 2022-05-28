using System.Collections.Generic;
using System.Linq;
using Tekla.Structures.Geometry3d;
using Tekla.Structures.Model;
using Tekla.Structures.Solid;
using tsm = Tekla.Structures.Model;

namespace RedButton.Common.TeklaStructures.Extensions
{
    public static class PointExtension
    {
        /// <summary>
        /// Getting the point of intersection of an array of line segments
        /// </summary>
        /// <param name="lines"></param>
        /// <returns></returns>
        public static Point GetIntersectedPoint(this List<LineSegment> lines)
        {
            List<LineSegment> copyLines = new List<LineSegment>(); 
            copyLines.AddRange(lines);
            List<Point> points = new List<Point>();
            for (int i = 0; i < copyLines.Count - 1; i++)
            {
                var l1 = lines[i];
                for (int k = 0; k < copyLines.Count - 1; k++)
                {
                    if (i == k) continue;
                    var l2 = lines[k];
                    var intersected = Intersection.LineToLine(new Line(l1), new Line(l2));
                    if (intersected != null)
                    {
                        points.Add(intersected.StartPoint);
                        points.Add(intersected.EndPoint);
                    }
                }
                copyLines.RemoveAt(i);
                i--;
            }

            Point result = points.First();
            var centerPoint = points.GetCenterPoint();
            double dist = Distance.PointToPoint(result, centerPoint);
            foreach (var p in points)
            {
                double _dist = Distance.PointToPoint(p, centerPoint);
                if (_dist < dist)
                {
                    result = p.NewPoint();
                    dist = _dist;
                }
            }

            return result;
        }
        
        /// <summary>
        /// Get center point between two points
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static Point GetCenterPoint(this Point p1, Point p2)
        {
            return new Point
            {
                X = (p2.X + p1.X) * 0.5,
                Y = (p2.Y + p1.Y) * 0.5,
                Z = (p2.Z + p1.Z) * 0.5
            };
        }
        
        /// <summary>
        /// Get center point from line segment
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public static Point GetCenterPoint(this LineSegment line)
        {
            return line.StartPoint.GetCenterPoint(line.EndPoint);
        }
        
        /// <summary>
        /// Get center point from array points
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        public static Point GetCenterPoint(this List<Point> points)
        {
            double x = 0, y = 0, z = 0;
            foreach (var p in points)
            {
                x += p.X;
                y += p.Y;
                z += p.Z;
            }
            return new Point(x / points.Count, y / points.Count, z / points.Count);
        }
        
        /// <summary>
        /// Creating a new point instance 
        /// </summary>
        /// <param name="p1"></param>
        /// <returns></returns>
        public static Point NewPoint(this Point p1)
        {
            return new Point(p1.X, p1.Y, p1.Z);
        }
        
        /// <summary>
        /// Creating a new point instance with vector addition
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public static Point NewPoint(this Point p1, Vector v1)
        {
            return new Point(p1.X + v1.X, p1.Y + v1.Y, p1.Z + v1.Z);
        }

        /// <summary>
        /// Point transformation from a first transformation plane to a second transformation plane
        /// </summary>
        /// <param name="point">Point</param>
        /// <param name="from">A first transformation plane</param>
        /// <param name="to">A second transformation plane</param>
        /// <returns></returns>
        public static Point Transformation(this Point point, TransformationPlane from, TransformationPlane to)
        {
            return to.TransformationMatrixToLocal.Transform(from.TransformationMatrixToGlobal.Transform(point));
        }
    }
}