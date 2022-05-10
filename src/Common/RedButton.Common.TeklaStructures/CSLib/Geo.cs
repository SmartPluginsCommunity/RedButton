using System;
using System.Collections.Generic;
using Tekla.Structures.Geometry3d;
using Tekla.Structures.Model;

namespace RedButton.Common.TeklaStructures.CSLib
{
    public static class Geo
    {
        private static Point comparePoint = new Point(0.0, 0.0, 0.0);

        public static bool CompareTwoLinesSegment2D(LineSegment lineSegment1, LineSegment lineSegment2)
        {
            bool flag = CompareTwoPoints2D(lineSegment1.Point1, lineSegment2.Point1) && CompareTwoPoints2D(lineSegment1.Point2, lineSegment2.Point2);
            if (!flag)
                flag = CompareTwoPoints2D(lineSegment1.Point1, lineSegment2.Point2) && CompareTwoPoints2D(lineSegment1.Point2, lineSegment2.Point1);
            return flag;
        }

        public static bool CompareTwoLinesSegment3D(LineSegment lineSegment1, LineSegment lineSegment2)
        {
            bool flag = CompareTwoPoints3D(lineSegment1.Point1, lineSegment2.Point1) && CompareTwoPoints3D(lineSegment1.Point2, lineSegment2.Point2);
            if (!flag)
                flag = CompareTwoPoints3D(lineSegment1.Point1, lineSegment2.Point2) && CompareTwoPoints3D(lineSegment1.Point2, lineSegment2.Point1);
            return flag;
        }

        public static bool CompareTwoPoints2D(Point point1, Point point2) => Compare.EQ(point1.X, point2.X) && Compare.EQ(point1.Y, point2.Y);

        public static bool CompareTwoPoints3D(Point point1, Point point2) => Compare.EQ(point1.X, point2.X) && Compare.EQ(point1.Y, point2.Y) && Compare.EQ(point1.Z, point2.Z);

        public static List<Point> ConvertListPointsFromPolygon(Polygon polygon)
        {
            List<Point> pointList = new List<Point>(polygon.Points.Count);
            foreach (Point point in polygon.Points)
                pointList.Add(new Point(point));
            return pointList;
        }

        
        public static Polygon ConvertPolygonFromListPoint(List<Point> polygonPoints)
        {
            Polygon polygon = new Polygon();
            foreach (Point polygonPoint in polygonPoints)
                polygon.Points.Add((object)new Point(polygonPoint));
            return polygon;
        }
        
        public static List<Point> ListOfPointsFromPolygon(this Polygon polygon)
        {
            List<Point> pointList = new List<Point>(polygon.Points.Count);
            foreach (Point point in polygon.Points)
                pointList.Add(new Point(point));
            return pointList;
        }
        
        public static double GetAngle3D(Point center, Point point1, Point point2)
        {
            if (CompareTwoPoints3D(point1, point2))
                return 0.0;
            Vector vectorLineSegment1 = GetVectorLineSegment(point1, center);
            Vector vectorLineSegment2 = GetVectorLineSegment(point2, center);
            double d = Math.Acos((vectorLineSegment1.X * vectorLineSegment2.X + vectorLineSegment1.Y * vectorLineSegment2.Y + vectorLineSegment1.Z * vectorLineSegment2.Z) / (vectorLineSegment1.GetLength() * vectorLineSegment2.GetLength()));
            if (double.IsNaN(d))
                d = 0.0;
            return d;
        }

        public static void ConvexHull(ref List<Point> convexHull)
        {
            List<Point> convexHull1 = new List<Point>();
            if (convexHull.Count < 3)
                return;
            foreach (Point Point in convexHull)
                convexHull1.Add(new Point(Point));
            convexHull.Clear();
            ConvexHullSummary(ref convexHull1);
            convexHull.AddRange((IEnumerable<Point>)convexHull1);
        }
        
        public static void ConvexHull(ref List<Point> convexHull, bool deleteCollinearPoints = true)
        {
            ConvexHullSummary(ref convexHull);
            if (!(convexHull.Count > 2 & deleteCollinearPoints))
                return;
            for (int index = 0; index < convexHull.Count; ++index)
            {
                if (IsPointsCollinear(convexHull[index], convexHull[(index + 1) % convexHull.Count], convexHull[(index + 2) % convexHull.Count]))
                {
                    convexHull.RemoveAt((index + 1) % convexHull.Count);
                    --index;
                }
            }
        }

        public static void CopyPointPosition(Point pointToCopyTo, Point orginalPoint)
        {
            pointToCopyTo.X = orginalPoint.X;
            pointToCopyTo.Y = orginalPoint.Y;
            pointToCopyTo.Z = orginalPoint.Z;
        }

        public static List<LineSegment> CutLineByPolygon2D(
          LineSegment lineSegment,
          List<Point> polygon,
          bool returnInside)
        {
            List<LineSegment> lineSegmentList = new List<LineSegment>();
            List<Point> polygon1 = new List<Point>(polygon.Count);
            foreach (Point Point in polygon)
                polygon1.Add(new Point(Point)
                {
                    Z = lineSegment.Point1.Z
                });
            List<Point> pointList = new List<Point>();
            for (int index1 = 0; index1 < polygon1.Count; ++index1)
            {
                int index2 = index1 + 1;
                if (index2 >= polygon1.Count)
                    index2 = 0;
                LineSegment lineSegment2 = new LineSegment(new Point(polygon1[index1]), new Point(polygon1[index2]));
                List<Point> intersectPoints = new List<Point>();
                Intersect.IntersectLineSegmentToLineSegment2D(lineSegment, lineSegment2, ref intersectPoints);
                pointList.AddRange((IEnumerable<Point>)intersectPoints);
            }
            if (IsPointInsidePolygon2D(polygon1, lineSegment.Point1, true) == returnInside)
                pointList.Add(new Point(lineSegment.Point1));
            if (IsPointInsidePolygon2D(polygon1, lineSegment.Point2, true) == returnInside)
                pointList.Add(new Point(lineSegment.Point2));
            comparePoint = lineSegment.Point1;
            pointList.Sort(new Comparison<Point>(ICompareTwoPoints2D));
            comparePoint = new Point(0.0, 0.0, 0.0);
            for (int index1 = 0; index1 < pointList.Count; ++index1)
            {
                for (int index2 = 0; index2 < pointList.Count; ++index2)
                {
                    if (index1 != index2 && CompareTwoPoints2D(pointList[index1], pointList[index2]))
                    {
                        pointList.RemoveAt(index2);
                        --index2;
                    }
                }
            }
            for (int index1 = 0; index1 < pointList.Count; ++index1)
            {
                int index2 = index1 + 1;
                if (index2 < pointList.Count)
                    lineSegmentList.Add(new LineSegment(pointList[index1], pointList[index2]));
                else
                    break;
            }
            for (int index = 0; index < lineSegmentList.Count; ++index)
            {
                Point centerPoint2D = GetCenterPoint2D(lineSegmentList[index].Point1, lineSegmentList[index].Point2);
                if (IsPointInsidePolygon2D(polygon1, centerPoint2D, true) != returnInside)
                {
                    lineSegmentList.RemoveAt(index);
                    --index;
                }
            }
            for (int index1 = 0; index1 < lineSegmentList.Count; ++index1)
            {
                int index2 = index1 + 1;
                if (index2 < lineSegmentList.Count)
                {
                    if (CompareTwoPoints2D(lineSegmentList[index1].Point2, lineSegmentList[index2].Point1))
                    {
                        lineSegmentList[index1].Point2 = lineSegmentList[index2].Point2;
                        lineSegmentList.RemoveAt(index2);
                        --index1;
                    }
                }
                else
                    break;
            }
            return lineSegmentList;
        }

        public static List<LineSegment> CutLineByPolygon2D(
          LineSegment lineSegment,
          Polygon polygon,
          bool returnInside)
        {
            List<Point> polygon1 = ConvertListPointsFromPolygon(polygon);
            return CutLineByPolygon2D(lineSegment, polygon1, returnInside);
        }

        public static Point GetCenterPoint2D(Point point1, Point point2)
        {
            Point point = new Point(point1);
            Vector vectorLineSegment = GetVectorLineSegment(point2, point1);
            point.X += vectorLineSegment.X * 0.5;
            point.Y += vectorLineSegment.Y * 0.5;
            point.Z = 0.0;
            return point;
        }

        public static Point GetCenterPoint3D(Point point1, Point point2)
        {
            Point point = new Point(point1);
            Vector vectorLineSegment = GetVectorLineSegment(point2, point1);
            point.X += vectorLineSegment.X * 0.5;
            point.Y += vectorLineSegment.Y * 0.5;
            point.Z += vectorLineSegment.Z * 0.5;
            return point;
        }

        public static double GetDistanceBeetveenTwoPoints2D(Point point1, Point point2) => Math.Sqrt((point2.X - point1.X) * (point2.X - point1.X) + (point2.Y - point1.Y) * (point2.Y - point1.Y));

        public static double GetDistanceBeetveenTwoPoints3D(Point point1, Point point2) => Math.Sqrt((point2.X - point1.X) * (point2.X - point1.X) + (point2.Y - point1.Y) * (point2.Y - point1.Y) + (point2.Z - point1.Z) * (point2.Z - point1.Z));

        public static double GetDistanceBetweenTwoLineSegments3D(
          Point line11,
          Point line12,
          Point line21,
          Point line22)
        {
            double b = GetDistanceBeetveenTwoPoints3D(line11, line21);
            double beetveenTwoPoints3D1 = GetDistanceBeetveenTwoPoints3D(line11, line22);
            if (Compare.LT(beetveenTwoPoints3D1, b))
                b = beetveenTwoPoints3D1;
            double beetveenTwoPoints3D2 = GetDistanceBeetveenTwoPoints3D(line12, line21);
            if (Compare.LT(beetveenTwoPoints3D2, b))
                b = beetveenTwoPoints3D2;
            double beetveenTwoPoints3D3 = GetDistanceBeetveenTwoPoints3D(line12, line22);
            if (Compare.LT(beetveenTwoPoints3D3, b))
                b = beetveenTwoPoints3D3;
            Line Line1 = new Line(line11, line12);
            Line Line2 = new Line(line21, line22);
            Point line1 = Projection.PointToLine(line11, Line2);
            if (IsPointInLineSegment2D(line21, line22, line1))
            {
                double beetveenTwoPoints3D4 = GetDistanceBeetveenTwoPoints3D(line1, line11);
                if (Compare.LT(beetveenTwoPoints3D4, b))
                    b = beetveenTwoPoints3D4;
            }
            Point line2 = Projection.PointToLine(line12, Line2);
            if (IsPointInLineSegment2D(line21, line22, line2))
            {
                double beetveenTwoPoints3D4 = GetDistanceBeetveenTwoPoints3D(line2, line12);
                if (Compare.LT(beetveenTwoPoints3D4, b))
                    b = beetveenTwoPoints3D4;
            }
            Point line3 = Projection.PointToLine(line21, Line1);
            if (IsPointInLineSegment2D(line11, line12, line3))
            {
                double beetveenTwoPoints3D4 = GetDistanceBeetveenTwoPoints3D(line3, line21);
                if (Compare.LT(beetveenTwoPoints3D4, b))
                    b = beetveenTwoPoints3D4;
            }
            Point line4 = Projection.PointToLine(line22, Line1);
            if (IsPointInLineSegment2D(line11, line12, line4))
            {
                double beetveenTwoPoints3D4 = GetDistanceBeetveenTwoPoints3D(line4, line22);
                if (Compare.LT(beetveenTwoPoints3D4, b))
                    b = beetveenTwoPoints3D4;
            }
            List<Point> intersectPoints = new List<Point>();
            if (Intersect.IntersectLineSegmentToLineSegment3D(line11, line12, line21, line22, ref intersectPoints))
                b = 0.0;
            return b;
        }

        public static double GetDistancePointFromLine(Point point, Line line) => GetDistanceBeetveenTwoPoints3D(Projection.PointToLine(point, line), point);

        public static double GetDistancePointFromLine(Point point, Point linePoint1, Point linePoint2) => GetDistancePointFromLine(point, new Line(linePoint1, linePoint2));

        public static Vector GetNormalVectorInPlane(
          Vector vector,
          Vector vectorToDefinePlaneAndDirection)
        {
            Vector vector1 = new Vector();
            return vector.Cross(vectorToDefinePlaneAndDirection).Cross(vector);
        }

        public static Vector GetVectorLineSegment(LineSegment lineSegment) => GetVectorLineSegment(lineSegment.Point1, lineSegment.Point2);

        public static Vector GetVectorLineSegment(Point point1, Point point2) => new Vector(point1.X - point2.X, point1.Y - point2.Y, point1.Z - point2.Z);

        public static int ICompareTwoPoints2D(Point point1, Point point2) => Convert.ToInt32(GetDistanceBeetveenTwoPoints2D(point1, comparePoint) - GetDistanceBeetveenTwoPoints2D(point2, comparePoint));

        public static int ICompareTwoPoints3D(Point point1, Point point2) => Convert.ToInt32(GetDistanceBeetveenTwoPoints3D(point1, comparePoint) - GetDistanceBeetveenTwoPoints3D(point2, comparePoint));

        public static bool IsPointInLineSegment2D(Point point1, Point point2, Point testPoint) => IsPointInLineSegment2D(point1, point2, testPoint, 20.0 * Constants.CS_EPSILON);

        public static bool IsPointInLineSegment2D(
          Point point1,
          Point point2,
          Point testPoint,
          double minimalDistance)
        {
            bool flag = false;
            if (Compare.LE(GetDistanceBeetveenTwoPoints2D(point1, testPoint) + GetDistanceBeetveenTwoPoints2D(point2, testPoint) - GetDistanceBeetveenTwoPoints2D(point1, point2), minimalDistance))
                flag = Compare.LE(GetDistanceBeetveenTwoPoints2D(Projection.PointToLine(testPoint, new Line(point1, point2)), testPoint), minimalDistance);
            return flag;
        }

        public static bool IsPointInLineSegment3D(Point point1, Point point2, Point testPoint) => IsPointInLineSegment3D(point1, point2, testPoint, 20.0 * Constants.CS_EPSILON);

        public static bool IsPointInLineSegment3D(
          Point point1,
          Point point2,
          Point testPoint,
          double minimalDistance)
        {
            bool flag = false;
            if (Compare.LT(GetDistanceBeetveenTwoPoints3D(point1, testPoint) + GetDistanceBeetveenTwoPoints3D(point2, testPoint) - GetDistanceBeetveenTwoPoints3D(point1, point2), minimalDistance))
                flag = Compare.LE(GetDistanceBeetveenTwoPoints3D(Projection.PointToLine(testPoint, new Line(point1, point2)), testPoint), minimalDistance);
            return flag;
        }

        public static bool IsPointInsideBoundingBox(
          Point point,
          Point boundingBoxMinimalPoint,
          Point boundingBoxMaximalPoint)
        {
            bool flag = false;
            if (point != (Point)null && boundingBoxMinimalPoint != (Point)null && (boundingBoxMaximalPoint != (Point)null && Compare.GE(point.X, boundingBoxMinimalPoint.X)) && (Compare.LE(point.X, boundingBoxMaximalPoint.X) && Compare.GE(point.Y, boundingBoxMinimalPoint.Y) && (Compare.LE(point.Y, boundingBoxMaximalPoint.Y) && Compare.GE(point.Z, boundingBoxMinimalPoint.Z))) && Compare.LE(point.Z, boundingBoxMaximalPoint.Z))
                flag = true;
            return flag;
        }

        public static bool IsPointInsidePolygon2D(Polygon polygon, Point testPoint) => IsPointInsidePolygon2D(ConvertListPointsFromPolygon(polygon), testPoint);

        public static bool IsPointInsidePolygon2D(Polygon polygon, Point testPoint, bool withBorder) => IsPointInsidePolygon2D(ConvertListPointsFromPolygon(polygon), testPoint, withBorder);

        public static bool IsPointInsidePolygon2D(List<Point> polygonPoints, Point testPoint)
        {
            int index1 = 0;
            bool flag = false;
            for (int index2 = 0; index2 < polygonPoints.Count; ++index2)
            {
                ++index1;
                if (index1 == polygonPoints.Count)
                    index1 = 0;
                if (Compare.LT(polygonPoints[index2].Y, testPoint.Y) && Compare.GE(polygonPoints[index1].Y, testPoint.Y) || Compare.LT(polygonPoints[index1].Y, testPoint.Y) && Compare.GE(polygonPoints[index2].Y, testPoint.Y))
                {
                    double num1 = testPoint.Y - polygonPoints[index2].Y;
                    double num2 = polygonPoints[index1].Y - polygonPoints[index2].Y;
                    double num3 = polygonPoints[index1].X - polygonPoints[index2].X;
                    if (Compare.LT(polygonPoints[index2].X + num1 / num2 * num3, testPoint.X))
                        flag = !flag;
                }
            }
            return flag;
        }

        public static bool IsPointInsidePolygon2D(
          List<Point> polygon,
          Point testPoint,
          bool withBorder)
        {
            int index1 = 0;
            for (int index2 = 0; index2 < polygon.Count; ++index2)
            {
                ++index1;
                if (index1 == polygon.Count)
                    index1 = 0;
                if (IsPointInLineSegment2D(polygon[index2], polygon[index1], testPoint))
                    return withBorder;
            }
            return IsPointInsidePolygon2D(polygon, testPoint);
        }

        public static Point MovePointOnLine(Point pointToMove, Point linePoint, double distance)
        {
            Point point2 = new Point(pointToMove);
            Vector normal = GetVectorLineSegment(linePoint, point2).GetNormal();
            point2.Z += normal.Z * distance;
            point2.Y += normal.Y * distance;
            point2.X += normal.X * distance;
            return point2;
        }

        public static void PointParallel(
          ref Point p1,
          ref Point p2,
          Point inputPoint1,
          Point inputPoint2,
          double actualOffset)
        {
            if (inputPoint1 == (Point)null && inputPoint2 == (Point)null)
                return;
            if (p1 == (Point)null)
                p1 = new Point();
            if (p2 == (Point)null)
                p2 = new Point();
            if (inputPoint1 == (Point)null)
                inputPoint1 = new Point();
            if (inputPoint2 == (Point)null)
                inputPoint2 = new Point();
            if (actualOffset == 0.0)
            {
                p1 = new Point(inputPoint1);
                p2 = new Point(inputPoint2);
            }
            else
            {
                Vector Vector = new Vector(inputPoint2.X - inputPoint1.X, inputPoint2.Y - inputPoint1.Y, inputPoint2.Z - inputPoint1.Z);
                Vector vector = new Vector(0.0, 0.0, -1000.0);
                vector.Normalize();
                Vector.Normalize();
                CoordinateSystem newSystem = new CoordinateSystem();
                newSystem.Origin = inputPoint1;
                newSystem.AxisX = vector.Cross(Vector);
                newSystem.AxisY = Vector;
                SetPlane setPlane = new SetPlane(new Tekla.Structures.Model.Model());
                setPlane.AddPoints(inputPoint1);
                setPlane.AddPoints(inputPoint2);
                setPlane.AddPoints(p1);
                setPlane.AddPoints(p2);
                setPlane.Begin(newSystem);
                try
                {
                    p1.X = inputPoint1.X + actualOffset;
                    p1.Y = inputPoint1.Y;
                    p1.Z = inputPoint1.Z;
                    p2.X = inputPoint2.X + actualOffset;
                    p2.Y = inputPoint2.Y;
                    p2.Z = inputPoint2.Z;
                }
                catch
                {
                }
                setPlane.End();
            }
        }

        public static void SortPoints2D(List<Point> points, Point mainPoint)
        {
            comparePoint = mainPoint;
            points.Sort(new Comparison<Point>(ICompareTwoPoints2D));
            comparePoint = new Point(0.0, 0.0, 0.0);
        }

        public static void SortPoints3D(List<Point> points, Point mainPoint)
        {
            comparePoint = mainPoint;
            points.Sort(new Comparison<Point>(ICompareTwoPoints3D));
            comparePoint = new Point(0.0, 0.0, 0.0);
        }

        public static double VectorAngle(Point p1, Point p2, Point p3, Point p4) => Math.Acos(VectorDotProduct(p1, p2, p3, p4) / (GetDistanceBeetveenTwoPoints3D(p1, p2) * GetDistanceBeetveenTwoPoints3D(p3, p4)));

        public static double VectorDotProduct(Point p1, Point p2, Point p3, Point p4) => DX(p2, p1) * DX(p4, p3) + DY(p2, p1) * DY(p4, p3) + DZ(p2, p1) * DZ(p4, p3);

        private static void ConvexHullSummary(ref List<Point> convexHull)
        {
            int index1 = 0;
            int index2 = 0;
            List<int> intList = new List<int>();
            List<Point> pointList1 = new List<Point>();
            pointList1.AddRange((IEnumerable<Point>)convexHull);
            List<Point> pointList2 = new List<Point>();
            if (convexHull.Count < 3)
                return;
            convexHull.Clear();
            for (int index3 = 1; index3 < pointList1.Count; ++index3)
            {
                if (Compare.GT(pointList1[index3].Y, pointList1[index1].Y))
                    index1 = index3;
                if (Compare.LT(pointList1[index3].Y, pointList1[index2].Y))
                    index2 = index3;
            }
            convexHull.Add(pointList1[index2]);
            intList.Add(index2);
            int index4 = index2;
            while (index4 != index1)
            {
                int index3 = index4;
                for (int index5 = 0; index5 < pointList1.Count; ++index5)
                {
                    if (Compare.LE(FindRelativeAngle(pointList1[index4], pointList1[index3]), FindRelativeAngle(pointList1[index4], pointList1[index5])) && (!intList.Contains(index5) || index5 == index1) && Compare.LE(FindRelativeAngle(pointList1[index4], pointList1[index5]), 180.0))
                        index3 = index5;
                }
                index4 = index3;
                convexHull.Add(pointList1[index4]);
                intList.Add(index4);
            }
            int index6 = index2;
            while (index6 != index1)
            {
                int index3 = index1;
                for (int index5 = 0; index5 < pointList1.Count; ++index5)
                {
                    if (Compare.GE(FindRelativeAngle(pointList1[index6], pointList1[index3]), FindRelativeAngle(pointList1[index6], pointList1[index5])) && (!intList.Contains(index5) || index5 == index1) && Compare.LE(FindRelativeAngle(pointList1[index6], pointList1[index5]), 180.0))
                        index3 = index5;
                }
                index6 = index3;
                pointList2.Add(pointList1[index6]);
                intList.Add(index6);
            }
            if (pointList2.Count > 0)
            {
                pointList2.Reverse();
                pointList2.RemoveAt(0);
                convexHull.AddRange((IEnumerable<Point>)pointList2);
            }
            if (convexHull.Count <= 2)
                return;
            for (int index3 = 0; index3 < convexHull.Count; ++index3)
            {
                if (IsPointsCollinear(convexHull[index3], convexHull[(index3 + 1) % convexHull.Count], convexHull[(index3 + 2) % convexHull.Count]))
                {
                    convexHull.RemoveAt((index3 + 1) % convexHull.Count);
                    --index3;
                }
            }
        }

        private static double DX(Point p1, Point p2) => p2.X - p1.X;

        private static double DY(Point p1, Point p2) => p2.Y - p1.Y;

        private static double DZ(Point p1, Point p2) => p2.Z - p1.Z;

        private static double FindRelativeAngle(Point point1, Point point2)
        {
            double num1 = point2.X - point1.X;
            double num2 = point2.Y - point1.Y;
            double a;
            if (Compare.EQ(num1, 0.0) && Compare.EQ(num2, 0.0))
            {
                a = 0.0;
            }
            else
            {
                a = Constants.RAD_TO_DEG * Math.Atan2(num2, num1);
                if (Compare.LT(a, 0.0))
                    a += 360.0;
            }
            return a;
        }

        private static bool IsPointsCollinear(Point point1, Point point2, Point point3)
        {
            double beetveenTwoPoints3D1 = GetDistanceBeetveenTwoPoints3D(point1, point2);
            double beetveenTwoPoints3D2 = GetDistanceBeetveenTwoPoints3D(point2, point3);
            double beetveenTwoPoints3D3 = GetDistanceBeetveenTwoPoints3D(point1, point3);
            return Compare.EQ(beetveenTwoPoints3D1 + beetveenTwoPoints3D2, beetveenTwoPoints3D3) || Compare.EQ(beetveenTwoPoints3D2 + beetveenTwoPoints3D3, beetveenTwoPoints3D1) || Compare.EQ(beetveenTwoPoints3D1 + beetveenTwoPoints3D3, beetveenTwoPoints3D2);
        }
    }
}