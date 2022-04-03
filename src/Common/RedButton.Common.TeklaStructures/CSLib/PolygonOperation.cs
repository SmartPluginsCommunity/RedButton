using System;
using System.Collections;
using System.Collections.Generic;
using Tekla.Structures.Geometry3d;
using Tekla.Structures.Model;

namespace RedButton.Common.TeklaStructures.CSLib
{
    public class PolygonOperation
    {
        public static bool GetPolygonOrientation(Polygon polygon) => PolygonOperation.GetPolygonOrientation(Geo.ConvertListPointsFromPolygon(polygon));

        public static bool GetPolygonOrientation(List<Point> polygon)
        {
            int index1 = 0;
            int index2 = index1 + 1;
            double a = 0.0;
            while (index1 < polygon.Count)
            {
                Point point1 = polygon[index1];
                Point point2 = polygon[index2];
                a += (point2.X - point1.X) * (point2.Y + point1.Y);
                ++index1;
                ++index2;
                if (index2 >= polygon.Count)
                    index2 -= polygon.Count;
            }
            return Compare.GT(a, 0.0);
        }

        public static void PolygonOffset(
          Polygon polygon,
          List<double> offsets,
          bool negative,
          bool openPolygon)
        {
            if (offsets == null || offsets.Count < 1 || (polygon == null || polygon.Points.Count < 1))
                return;
            Polygon polygon1 = new Polygon();
            foreach (Point point in polygon.Points)
                polygon1.Points.Add((object)new Point(point));
            bool flag = false;
            int count = polygon.Points.Count;
            if (Geo.CompareTwoPoints3D(polygon.Points[0] as Point, polygon.Points[count - 1] as Point))
            {
                --count;
                flag = true;
            }
            double offset = offsets[0];
            for (int index = 0; index < count; ++index)
            {
                if (index < offsets.Count)
                    offset = offsets[index];
                else
                    offsets.Add(offset);
            }
            int num = count;
            if (openPolygon)
                --num;
            for (int index = 0; index < num; ++index)
            {
                Vector Vector = new Vector((polygon.Points[(index + 1) % count] as Point).X - (polygon.Points[index % count] as Point).X, (polygon.Points[(index + 1) % count] as Point).Y - (polygon.Points[index % count] as Point).Y, (polygon.Points[(index + 1) % count] as Point).Z - (polygon.Points[index % count] as Point).Z);
                Vector vector = new Vector(0.0, 0.0, -1000.0);
                vector.Normalize();
                Vector.Normalize();
                CoordinateSystem newSystem = new CoordinateSystem();
                newSystem.Origin = polygon.Points[index] as Point;
                newSystem.AxisX = vector.Cross(Vector);
                newSystem.AxisY = Vector;
                SetPlane setPlane = new SetPlane(new Tekla.Structures.Model.Model());
                setPlane.AddPolygons(polygon);
                setPlane.AddPolygons(polygon1);
                setPlane.Begin(newSystem);
                try
                {
                    Point p1 = new Point();
                    Point p2 = new Point();
                    Point Point = new Point();
                    double actualOffset = offsets[index];
                    Point testPoint = new Point(((polygon.Points[index] as Point).X + (polygon.Points[(index + 1) % count] as Point).X) / 2.0, ((polygon.Points[index] as Point).Y + (polygon.Points[(index + 1) % count] as Point).Y) / 2.0 + 1.0, ((polygon.Points[index] as Point).Z + (polygon.Points[(index + 1) % count] as Point).Z) / 2.0);
                    if (!Geo.IsPointInsidePolygon2D(polygon, testPoint, false))
                        actualOffset = -actualOffset;
                    if (negative)
                        actualOffset = -actualOffset;
                    Geo.PointParallel(ref p1, ref p2, polygon.Points[index] as Point, polygon.Points[(index + 1) % count] as Point, actualOffset);
                    if (index == 0)
                    {
                        polygon1.Points[index] = (object)p1;
                        polygon1.Points[index + 1] = (object)p2;
                    }
                    else if (index == num - 1)
                    {
                        if (Distance.PointToPoint(polygon1.Points[index] as Point, p1) == 0.0)
                        {
                            Point = new Point(p1);
                        }
                        else
                        {
                            LineSegment line = Intersection.LineToLine(new Line(polygon1.Points[index - 1] as Point, polygon1.Points[index] as Point), new Line(p1, p2));
                            if (line != (LineSegment)null && line.Point1 != (Point)null)
                                Point = new Point(line.Point1);
                        }
                        polygon1.Points[index] = (object)new Point(Point);
                        if (!openPolygon)
                        {
                            LineSegment line = Intersection.LineToLine(new Line(polygon1.Points[0] as Point, polygon1.Points[1] as Point), new Line(p1, p2));
                            if (line != (LineSegment)null && line.Point1 != (Point)null)
                                Point = new Point(line.Point1);
                            polygon1.Points[0] = (object)new Point(Point);
                        }
                        else
                            polygon1.Points[index + 1] = (object)new Point(p2);
                    }
                    else
                    {
                        if (Distance.PointToPoint(polygon1.Points[index] as Point, p1) == 0.0)
                        {
                            Point = new Point(p1);
                        }
                        else
                        {
                            LineSegment line = Intersection.LineToLine(new Line(polygon1.Points[index - 1] as Point, polygon1.Points[index] as Point), new Line(p1, p2));
                            if (line != (LineSegment)null && line.Point1 != (Point)null)
                                Point = new Point(line.Point1);
                        }
                        polygon1.Points[index] = (object)new Point(Point);
                        polygon1.Points[index + 1] = (object)new Point(p2);
                    }
                }
                catch
                {
                }
                setPlane.End();
            }
            if (flag)
            {
                polygon1.Points[count] = (object)new Point(polygon1.Points[0] as Point);
                ++count;
            }
            for (int index = 0; index < count; ++index)
                polygon.Points[index] = (object)new Point(polygon1.Points[index] as Point);
        }

        public static List<Point> ResizePolygon(List<Point> inputPolygon, double offset)
        {
            List<Point> polygonPoints = new List<Point>(inputPolygon.Count);
            List<Line> lineList = new List<Line>();
            bool polygonOrientation1 = PolygonOperation.GetPolygonOrientation(inputPolygon);
            if (polygonOrientation1)
                inputPolygon.Reverse();
            for (int index1 = 0; index1 < inputPolygon.Count; ++index1)
            {
                int index2 = index1 + 1;
                if (index2 >= inputPolygon.Count)
                    index2 = 0;
                Vector vectorLineSegment = Geo.GetVectorLineSegment(inputPolygon[index2], inputPolygon[index1]);
                Vector newVectorY = new Vector(vectorLineSegment.Y, -vectorLineSegment.X, vectorLineSegment.Z);
                SetPlane setPlane = new SetPlane(new Tekla.Structures.Model.Model());
                setPlane.AddPoints(inputPolygon[index1], inputPolygon[index2]);
                setPlane.Begin(new Point(inputPolygon[index1]), vectorLineSegment, newVectorY);
                Point p1 = new Point(inputPolygon[index1].X, inputPolygon[index1].Y + offset, inputPolygon[index1].Z);
                Point p2 = new Point(inputPolygon[index2].X, inputPolygon[index2].Y + offset, inputPolygon[index2].Z);
                setPlane.AddPoints(p1, p2);
                setPlane.End();
                lineList.Add(new Line(p1, p2));
            }
            for (int index1 = 0; index1 < lineList.Count; ++index1)
            {
                int index2 = index1 + 1;
                if (index2 >= lineList.Count)
                    index2 = 0;
                LineSegment line = Intersection.LineToLine(lineList[index1], lineList[index2]);
                if (line != (LineSegment)null && Geo.CompareTwoPoints3D(line.Point1, line.Point2))
                    polygonPoints.Add(new Point(line.Point1));
            }
            List<Point> polygon1;
            if (polygonPoints.Count == inputPolygon.Count)
            {
                Point point1 = polygonPoints[polygonPoints.Count - 1];
                polygonPoints.RemoveAt(polygonPoints.Count - 1);
                polygonPoints.Insert(0, point1);
                Polygon polygon2 = Geo.ConvertPolygonFromListPoint(polygonPoints);
                Polygon polygon1_1 = new Polygon();
                Polygon polygon2_1 = new Polygon();
                foreach (Point point2 in polygon2.Points)
                    polygon1_1.Points.Add((object)new Point(point2));
                foreach (Point point2 in polygon2.Points)
                    polygon2_1.Points.Add((object)new Point(point2));
                PolygonOperation polygonOperation = new PolygonOperation();
                List<LineSegment> lines = new List<LineSegment>();
                polygonOperation.CsGetLinesList(polygon1_1, polygon2_1, lines, PolygonOperation.SelectionTypeEnum.GET_ALL_LINES);
                double b = Math.Abs(offset);
                for (int index1 = lines.Count - 1; index1 >= 0; --index1)
                {
                    for (int index2 = 0; index2 < inputPolygon.Count; ++index2)
                    {
                        int index3 = index2 + 1;
                        if (index3 >= inputPolygon.Count)
                            index3 -= inputPolygon.Count;
                        if (Compare.LT(Geo.GetDistanceBetweenTwoLineSegments3D(lines[index1].Point1, lines[index1].Point2, inputPolygon[index2], inputPolygon[index3]), b))
                        {
                            lines.RemoveAt(index1);
                            break;
                        }
                    }
                }
                List<Polygon> returnPolygons = new List<Polygon>();
                new PolygonOperation.CreatePolygons().Create(lines, ref returnPolygons);
                if (returnPolygons.Count == 1)
                {
                    polygon1 = Geo.ConvertListPointsFromPolygon(returnPolygons[0]);
                    PolygonOperation.RemoveUnnecessaryPointsFromPolygon(polygon1);
                    bool polygonOrientation2 = PolygonOperation.GetPolygonOrientation(polygon1);
                    if (polygonOrientation1 != polygonOrientation2)
                        polygon1.Reverse();
                }
                else
                    polygon1 = (List<Point>)null;
            }
            else
                polygon1 = (List<Point>)null;
            if (polygonOrientation1)
                inputPolygon.Reverse();
            return polygon1;
        }

        public static List<Point> ResizePolygon(List<Point> inputPolygon, List<double> offsets)
        {
            if (offsets == null || offsets.Count < 1 || (inputPolygon == null || inputPolygon.Count < 3))
                return (List<Point>)null;
            bool polygonOrientation1 = PolygonOperation.GetPolygonOrientation(inputPolygon);
            if (polygonOrientation1)
            {
                inputPolygon.Reverse();
                inputPolygon.Insert(0, inputPolygon[inputPolygon.Count - 1]);
                inputPolygon.RemoveAt(inputPolygon.Count - 1);
                offsets.Reverse();
            }
            List<Point> pointList = new List<Point>(inputPolygon.Count);
            List<Line> lineList = new List<Line>();
            List<double> doubleList = new List<double>();
            for (int index1 = 0; index1 < inputPolygon.Count; ++index1)
            {
                int index2 = index1 + 1;
                if (index2 >= inputPolygon.Count)
                    index2 = 0;
                double num = index1 >= offsets.Count ? offsets[offsets.Count - 1] : offsets[index1];
                Vector vectorLineSegment = Geo.GetVectorLineSegment(inputPolygon[index2], inputPolygon[index1]);
                Vector newVectorY = new Vector(vectorLineSegment.Y, -vectorLineSegment.X, vectorLineSegment.Z);
                SetPlane setPlane = new SetPlane(new Tekla.Structures.Model.Model());
                setPlane.AddPoints(inputPolygon[index1], inputPolygon[index2]);
                setPlane.Begin(new Point(inputPolygon[index1]), vectorLineSegment, newVectorY);
                Point p1 = new Point(inputPolygon[index1].X, inputPolygon[index1].Y + num, inputPolygon[index1].Z);
                Point p2 = new Point(inputPolygon[index2].X, inputPolygon[index2].Y + num, inputPolygon[index2].Z);
                setPlane.AddPoints(p1, p2);
                setPlane.End();
                lineList.Add(new Line(p1, p2));
                doubleList.Add(num);
            }
            for (int index1 = 0; index1 < lineList.Count; ++index1)
            {
                int index2 = index1 + 1;
                if (index2 >= lineList.Count)
                    index2 = 0;
                LineSegment line = Intersection.LineToLine(lineList[index1], lineList[index2]);
                if (line != (LineSegment)null && Geo.CompareTwoPoints3D(line.Point1, line.Point2))
                    pointList.Add(new Point(line.Point1));
            }
            List<Point> polygon;
            if (pointList.Count == inputPolygon.Count)
            {
                Point point = pointList[pointList.Count - 1];
                pointList.RemoveAt(pointList.Count - 1);
                pointList.Insert(0, point);
                List<LineSegment> lines1 = new List<LineSegment>();
                for (int index1 = 0; index1 < pointList.Count; ++index1)
                {
                    int index2 = index1 + 1;
                    if (index2 >= pointList.Count)
                        index2 = 0;
                    lines1.Add(new LineSegment(new Point(pointList[index1]), new Point(pointList[index2])));
                }
                List<LineSegment> lines2 = new List<LineSegment>();
                for (int index1 = 0; index1 < pointList.Count; ++index1)
                {
                    int index2 = index1 + 1;
                    if (index2 >= pointList.Count)
                        index2 = 0;
                    lines2.Add(new LineSegment(new Point(pointList[index1]), new Point(pointList[index2])));
                }
                List<LineSegment> lines3 = new List<LineSegment>();
                PolygonOperation.CsGetLinesList(lines1, lines2, lines3, PolygonOperation.SelectionTypeEnum.GET_ALL_LINES);
                for (int index1 = lines3.Count - 1; index1 >= 0; --index1)
                {
                    for (int index2 = 0; index2 < inputPolygon.Count; ++index2)
                    {
                        int index3 = index2 + 1;
                        if (index3 >= inputPolygon.Count)
                            index3 -= inputPolygon.Count;
                        double b = Math.Abs(doubleList[index2]);
                        if (Compare.LT(Geo.GetDistanceBetweenTwoLineSegments3D(lines3[index1].Point1, lines3[index1].Point2, inputPolygon[index2], inputPolygon[index3]), b))
                        {
                            Vector vectorLineSegment1 = Geo.GetVectorLineSegment(lines3[index1]);
                            Vector vectorLineSegment2 = Geo.GetVectorLineSegment(inputPolygon[index2], inputPolygon[index3]);
                            if (Compare.GT(vectorLineSegment1.Dot(vectorLineSegment2) / (vectorLineSegment1.GetLength() * vectorLineSegment2.GetLength()), 1.0 - Constants.CS_EPSILON))
                            {
                                lines3.RemoveAt(index1);
                                break;
                            }
                            break;
                        }
                    }
                }
                List<LineSegment> lines4 = new List<LineSegment>((IEnumerable<LineSegment>)lines3);
                List<Polygon> returnPolygons = new List<Polygon>();
                new PolygonOperation.CreatePolygons().Create(lines4, ref returnPolygons);
                if (returnPolygons.Count == 1)
                {
                    polygon = Geo.ConvertListPointsFromPolygon(returnPolygons[0]);
                    PolygonOperation.RemoveUnnecessaryPointsFromPolygon(polygon);
                    bool polygonOrientation2 = PolygonOperation.GetPolygonOrientation(polygon);
                    if (polygonOrientation1 != polygonOrientation2)
                        polygon.Reverse();
                }
                else
                    polygon = (List<Point>)null;
            }
            else
                polygon = (List<Point>)null;
            if (polygonOrientation1)
            {
                inputPolygon.Reverse();
                inputPolygon.Insert(0, inputPolygon[inputPolygon.Count - 1]);
                inputPolygon.RemoveAt(inputPolygon.Count - 1);
                offsets.Reverse();
            }
            return polygon;
        }

        public static double SummaryPolygonArea2D(Polygon polygon)
        {
            if (polygon.Points.Count < 3)
                return 0.0;
            List<Point> polygon1 = Geo.ConvertListPointsFromPolygon(polygon);
            polygon1.Add(new Point(polygon1[0]));
            return Math.Abs(PolygonOperation.SummaryPolygonArea2DStep(polygon1)) / 2.0;
        }

        public static double SummaryPolygonArea2D(List<Point> polygon)
        {
            if (polygon.Count < 3)
                return 0.0;
            return Math.Abs(PolygonOperation.SummaryPolygonArea2DStep(new List<Point>((IEnumerable<Point>)polygon)
      {
        new Point(polygon[0])
      })) / 2.0;
        }

        public PolygonOperation.ComparePolygonTypeEnum CsCmpTwoPolygons(
          Polygon polygon1,
          Polygon polygon2)
        {
            int index1 = 0;
            if (Compare.EQ(this.CsTwoPolygonsMinDist2D(polygon1, polygon2), 0.0))
            {
                if (polygon1.Points.Count == polygon2.Points.Count)
                {
                    bool flag = false;
                    int index2;
                    for (index2 = 0; index2 < polygon1.Points.Count; ++index2)
                    {
                        for (index1 = 0; index1 < polygon2.Points.Count; ++index1)
                        {
                            if (Geo.CompareTwoPoints2D(polygon1.Points[index2] as Point, polygon2.Points[index1] as Point))
                            {
                                flag = true;
                                break;
                            }
                        }
                        if (flag)
                            break;
                    }
                    if (index2 != polygon1.Points.Count || index1 != polygon2.Points.Count)
                    {
                        int num1 = 0;
                        while (num1 < polygon1.Points.Count && Geo.CompareTwoPoints2D(polygon1.Points[(num1 + index2) % polygon1.Points.Count] as Point, polygon2.Points[(num1 + index1) % polygon2.Points.Count] as Point))
                            ++num1;
                        if (num1 == polygon1.Points.Count)
                            return PolygonOperation.ComparePolygonTypeEnum.POL1_EQ_POL2;
                        int num2 = 0;
                        while (num2 < polygon1.Points.Count && Geo.CompareTwoPoints2D(polygon1.Points[(index2 + num2 + polygon1.Points.Count) % polygon1.Points.Count] as Point, polygon2.Points[(index1 - num2 + polygon2.Points.Count) % polygon2.Points.Count] as Point))
                            ++num2;
                        if (num2 == polygon1.Points.Count)
                            return PolygonOperation.ComparePolygonTypeEnum.POL1_EQ_POL2;
                    }
                }
                return PolygonOperation.ComparePolygonTypeEnum.POL1_COLLIDE_POL2;
            }
            int index3 = 0;
            while (index3 < polygon1.Points.Count && Geo.IsPointInsidePolygon2D(polygon2, polygon1.Points[index3] as Point, false))
                ++index3;
            if (index3 == polygon1.Points.Count)
                return PolygonOperation.ComparePolygonTypeEnum.POL1_IN_POL2;
            int index4 = 0;
            while (index4 < polygon2.Points.Count && Geo.IsPointInsidePolygon2D(polygon1, polygon2.Points[index4] as Point, false))
                ++index4;
            return index4 == polygon2.Points.Count ? PolygonOperation.ComparePolygonTypeEnum.POL2_IN_POL1 : PolygonOperation.ComparePolygonTypeEnum.POL_OUTSIDE;
        }

        public List<PolygonOperation.PolygonWithHoles> PolygonOperations(
          Polygon polygon1,
          Polygon polygon2,
          PolygonOperation.PolygonOperationsEnum operation)
        {
            Polygon polygon3 = new Polygon();
            Polygon polygon4 = new Polygon();
            foreach (Point point in polygon1.Points)
                polygon3.Points.Add((object)new Point(point));
            foreach (Point point in polygon2.Points)
                polygon4.Points.Add((object)new Point(point));
            List<PolygonOperation.PolygonWithHoles> resultPolygons = new List<PolygonOperation.PolygonWithHoles>();
            List<LineSegment> lines1 = new List<LineSegment>();
            List<LineSegment> lines2 = new List<LineSegment>();
            List<LineSegment> lines3 = new List<LineSegment>();
            List<LineSegment> lines4 = new List<LineSegment>();
            List<Polygon> returnPolygons1 = new List<Polygon>();
            PolygonOperation.CreatePolygons createPolygons = new PolygonOperation.CreatePolygons();
            switch (operation)
            {
                case PolygonOperation.PolygonOperationsEnum.UNION:
                    this.CsGetLinesList(polygon3, polygon4, lines1, PolygonOperation.SelectionTypeEnum.GET_ALL_LINES);
                    this.CsGetLinesList(polygon4, polygon3, lines1, PolygonOperation.SelectionTypeEnum.GET_ALL_LINES);
                    createPolygons.Create(lines1, ref returnPolygons1);
                    break;
                case PolygonOperation.PolygonOperationsEnum.DIFFERENCE:
                    this.CsGetLinesList(polygon3, polygon4, lines3, PolygonOperation.SelectionTypeEnum.GET_ALL_LINES);
                    goto default;
                default:
                    this.CsGetLinesList(polygon3, polygon4, lines2, PolygonOperation.SelectionTypeEnum.GET_INTERSECTION_LINES);
                    this.CsGetLinesList(polygon4, polygon3, lines2, PolygonOperation.SelectionTypeEnum.GET_INTERSECTION_LINES);
                    for (int index1 = 0; index1 < lines2.Count; ++index1)
                    {
                        for (int index2 = index1 + 1; index2 < lines2.Count; ++index2)
                        {
                            if (Geo.CompareTwoLinesSegment2D(lines2[index1], lines2[index2]))
                            {
                                lines2.RemoveAt(index2);
                                --index2;
                            }
                        }
                    }
                    switch (operation)
                    {
                        case PolygonOperation.PolygonOperationsEnum.INTERSECT:
                            createPolygons.Create(lines2, ref returnPolygons1);
                            break;
                        case PolygonOperation.PolygonOperationsEnum.DIFFERENCE:
                            List<Polygon> returnPolygons2 = new List<Polygon>();
                            createPolygons.Create(lines2, ref returnPolygons2);
                            for (int index = 0; index < returnPolygons2.Count; ++index)
                            {
                                polygon4.Points.Clear();
                                polygon4.Points.AddRange((ICollection)returnPolygons2[index].Points);
                                this.CsGetLinesList(polygon4, polygon3, lines2, PolygonOperation.SelectionTypeEnum.GET_INTERSECTION_LINES);
                            }
                            for (int index = 0; index < lines2.Count; ++index)
                            {
                                if (Geo.CompareTwoPoints2D(lines2[index].Point1, lines2[index].Point2))
                                {
                                    lines2.RemoveAt(index);
                                    --index;
                                }
                            }
                            for (int index1 = 0; index1 < lines2.Count; ++index1)
                            {
                                for (int index2 = index1 + 1; index2 < lines2.Count; ++index2)
                                {
                                    if (Geo.CompareTwoLinesSegment2D(lines2[index1], lines2[index2]))
                                    {
                                        lines2.RemoveAt(index2);
                                        --index2;
                                    }
                                }
                            }
                            for (int index1 = 0; index1 < lines3.Count; ++index1)
                            {
                                bool flag = false;
                                for (int index2 = 0; index2 < lines2.Count; ++index2)
                                {
                                    if (Geo.CompareTwoLinesSegment2D(lines3[index1], lines2[index2]))
                                    {
                                        flag = true;
                                        break;
                                    }
                                }
                                if (!flag)
                                    lines4.Add(lines3[index1]);
                            }
                            for (int index1 = 0; index1 < lines2.Count; ++index1)
                            {
                                bool flag = false;
                                for (int index2 = 0; index2 < lines3.Count; ++index2)
                                {
                                    if (Geo.CompareTwoLinesSegment2D(lines2[index1], lines3[index2]))
                                    {
                                        flag = true;
                                        break;
                                    }
                                }
                                if (!flag)
                                    lines4.Add(lines2[index1]);
                            }
                            createPolygons.Create(lines4, ref returnPolygons1);
                            break;
                    }
                    break;
            }
            this.SeparateHoles(returnPolygons1, resultPolygons);
            return resultPolygons;
        }

        private static void CsGetLinesList(
          List<LineSegment> lines1,
          List<LineSegment> lines2,
          List<LineSegment> lines,
          PolygonOperation.SelectionTypeEnum selectionType)
        {
            for (int index1 = 0; index1 < lines2.Count; ++index1)
            {
                List<LineSegment> lineSegmentList = new List<LineSegment>(lines1.Count);
                for (int index2 = 0; index2 < lines1.Count; ++index2)
                {
                    List<Point> intersectPoints = new List<Point>();
                    Intersect.IntersectLineSegmentToLineSegment2D(lines1[index2], lines2[index1], ref intersectPoints);
                    if (intersectPoints.Count == 1 && !Geo.CompareTwoPoints2D(lines1[index2].Point1, intersectPoints[0]) && !Geo.CompareTwoPoints2D(lines1[index2].Point2, intersectPoints[0]))
                    {
                        if (!Geo.CompareTwoPoints2D(lines1[index2].Point1, intersectPoints[0]))
                            lineSegmentList.Add(new LineSegment(new Point(lines1[index2].Point1), new Point(intersectPoints[0])));
                        if (!Geo.CompareTwoPoints2D(lines1[index2].Point2, intersectPoints[0]))
                            lineSegmentList.Add(new LineSegment(new Point(intersectPoints[0]), new Point(lines1[index2].Point2)));
                    }
                    else if (intersectPoints.Count == 2 && (!Geo.CompareTwoPoints2D(lines1[index2].Point1, intersectPoints[0]) || !Geo.CompareTwoPoints2D(lines1[index2].Point2, intersectPoints[1])))
                    {
                        if (!Geo.CompareTwoPoints2D(lines1[index2].Point1, intersectPoints[0]))
                            lineSegmentList.Add(new LineSegment(new Point(lines1[index2].Point1), new Point(intersectPoints[0])));
                        lineSegmentList.Add(new LineSegment(new Point(intersectPoints[0]), new Point(intersectPoints[1])));
                        if (!Geo.CompareTwoPoints2D(lines1[index2].Point2, intersectPoints[1]))
                            lineSegmentList.Add(new LineSegment(new Point(intersectPoints[1]), new Point(lines1[index2].Point2)));
                    }
                    else
                        lineSegmentList.Add(new LineSegment(new Point(lines1[index2].Point1), new Point(lines1[index2].Point2)));
                }
                lines1 = lineSegmentList;
            }
            foreach (LineSegment lineSegment in lines1)
                lines.Add(lineSegment);
        }

        private static void RemoveUnnecessaryPointsFromPolygon(List<Point> polygon)
        {
            for (int index1 = 0; index1 < polygon.Count; ++index1)
            {
                int index2 = index1 - 1;
                int index3 = index1 + 1;
                if (index2 < 0)
                    index2 += polygon.Count;
                if (index3 >= polygon.Count)
                    index3 -= polygon.Count;
                if (Geo.IsPointInLineSegment2D(polygon[index2], polygon[index3], polygon[index1]))
                {
                    polygon.RemoveAt(index1);
                    --index1;
                }
            }
        }

        private static double SummaryPolygonArea2DStep(List<Point> polygon)
        {
            if (polygon.Count < 2)
                return 0.0;
            double num = polygon[0].X * polygon[1].Y - polygon[1].X * polygon[0].Y;
            List<Point> polygon1 = new List<Point>((IEnumerable<Point>)polygon);
            polygon1.RemoveAt(0);
            return num + PolygonOperation.SummaryPolygonArea2DStep(polygon1);
        }

        private void CsGetLinesList(
          Polygon polygon1,
          Polygon polygon2,
          List<LineSegment> lines,
          PolygonOperation.SelectionTypeEnum selectionType)
        {
            List<LineSegment> lineSegmentList1 = new List<LineSegment>();
            List<LineSegment> lineSegmentList2 = new List<LineSegment>();
            for (int index1 = 0; index1 < polygon1.Points.Count; ++index1)
            {
                int index2 = index1 + 1;
                if (index2 >= polygon1.Points.Count)
                    index2 = 0;
                lineSegmentList1.Add(new LineSegment(new Point(polygon1.Points[index1] as Point), new Point(polygon1.Points[index2] as Point)));
            }
            for (int index1 = 0; index1 < polygon2.Points.Count; ++index1)
            {
                int index2 = index1 + 1;
                if (index2 >= polygon2.Points.Count)
                    index2 = 0;
                lineSegmentList2.Add(new LineSegment(new Point(polygon2.Points[index1] as Point), new Point(polygon2.Points[index2] as Point)));
            }
            for (int index1 = 0; index1 < lineSegmentList2.Count; ++index1)
            {
                List<LineSegment> lineSegmentList3 = new List<LineSegment>(lineSegmentList1.Count);
                for (int index2 = 0; index2 < lineSegmentList1.Count; ++index2)
                {
                    List<Point> intersectPoints = new List<Point>();
                    Intersect.IntersectLineSegmentToLineSegment2D(lineSegmentList1[index2], lineSegmentList2[index1], ref intersectPoints);
                    if (intersectPoints.Count == 1 && !Geo.CompareTwoPoints2D(lineSegmentList1[index2].Point1, intersectPoints[0]) && !Geo.CompareTwoPoints2D(lineSegmentList1[index2].Point2, intersectPoints[0]))
                    {
                        if (!Geo.CompareTwoPoints2D(lineSegmentList1[index2].Point1, intersectPoints[0]))
                            lineSegmentList3.Add(new LineSegment(new Point(lineSegmentList1[index2].Point1), new Point(intersectPoints[0])));
                        if (!Geo.CompareTwoPoints2D(lineSegmentList1[index2].Point2, intersectPoints[0]))
                            lineSegmentList3.Add(new LineSegment(new Point(intersectPoints[0]), new Point(lineSegmentList1[index2].Point2)));
                    }
                    else if (intersectPoints.Count == 2 && (!Geo.CompareTwoPoints2D(lineSegmentList1[index2].Point1, intersectPoints[0]) || !Geo.CompareTwoPoints2D(lineSegmentList1[index2].Point2, intersectPoints[1])))
                    {
                        if (!Geo.CompareTwoPoints2D(lineSegmentList1[index2].Point1, intersectPoints[0]))
                            lineSegmentList3.Add(new LineSegment(new Point(lineSegmentList1[index2].Point1), new Point(intersectPoints[0])));
                        lineSegmentList3.Add(new LineSegment(new Point(intersectPoints[0]), new Point(intersectPoints[1])));
                        if (!Geo.CompareTwoPoints2D(lineSegmentList1[index2].Point2, intersectPoints[1]))
                            lineSegmentList3.Add(new LineSegment(new Point(intersectPoints[1]), new Point(lineSegmentList1[index2].Point2)));
                    }
                    else
                        lineSegmentList3.Add(new LineSegment(new Point(lineSegmentList1[index2].Point1), new Point(lineSegmentList1[index2].Point2)));
                }
                lineSegmentList1 = lineSegmentList3;
            }
            if (selectionType == PolygonOperation.SelectionTypeEnum.GET_INTERSECTION_LINES)
            {
                for (int index = lineSegmentList1.Count - 1; index >= 0; --index)
                {
                    Point centerPoint2D = Geo.GetCenterPoint2D(lineSegmentList1[index].Point1, lineSegmentList1[index].Point2);
                    if (!Geo.IsPointInsidePolygon2D(polygon2, centerPoint2D, true))
                        lineSegmentList1.RemoveAt(index);
                }
            }
            foreach (LineSegment lineSegment in lineSegmentList1)
                lines.Add(lineSegment);
        }

        private double CsTwoPolygonsMinDist2D(Polygon polygon1, Polygon polygon2)
        {
            double num = double.MaxValue;
            LineSegment line1 = new LineSegment();
            LineSegment line2 = new LineSegment();
            for (int index1 = 0; index1 < polygon1.Points.Count; ++index1)
            {
                for (int index2 = 0; index2 < polygon2.Points.Count; ++index2)
                {
                    line1.Point1 = polygon1.Points[index1] as Point;
                    line1.Point2 = polygon1.Points[(index1 + 1) % polygon1.Points.Count] as Point;
                    line2.Point1 = polygon2.Points[index2] as Point;
                    line2.Point2 = polygon2.Points[(index2 + 1) % polygon2.Points.Count] as Point;
                    double distanceBetweenSegments = this.FindDistanceBetweenSegments(line1, line2);
                    if (num > distanceBetweenSegments)
                    {
                        num = distanceBetweenSegments;
                        if (num == 0.0)
                            return num;
                    }
                }
            }
            return num;
        }

        private double FindDistanceBetweenSegments(LineSegment line1, LineSegment line2)
        {
            bool segmentsIntersect;
            this.FindIntersection(line1, line2, out segmentsIntersect);
            if (segmentsIntersect)
                return 0.0;
            double num = double.MaxValue;
            double distanceToSegment1 = this.FindDistanceToSegment(line1.Point1, line2.Point1, line2.Point2);
            if (distanceToSegment1 < num)
                num = distanceToSegment1;
            double distanceToSegment2 = this.FindDistanceToSegment(line1.Point2, line2.Point1, line2.Point2);
            if (distanceToSegment2 < num)
                num = distanceToSegment2;
            double distanceToSegment3 = this.FindDistanceToSegment(line2.Point1, line1.Point1, line1.Point2);
            if (distanceToSegment3 < num)
                num = distanceToSegment3;
            double distanceToSegment4 = this.FindDistanceToSegment(line2.Point2, line1.Point1, line1.Point2);
            if (distanceToSegment4 < num)
                num = distanceToSegment4;
            return num;
        }

        private double FindDistanceToSegment(Point testPoint, Point point1, Point point2)
        {
            Point line = Projection.PointToLine(testPoint, new Line(point1, point2));
            return !Geo.IsPointInLineSegment2D(point1, point2, line) ? Math.Min(Geo.GetDistanceBeetveenTwoPoints2D(point1, testPoint), Geo.GetDistanceBeetveenTwoPoints2D(point2, testPoint)) : Geo.GetDistanceBeetveenTwoPoints2D(line, testPoint);
        }

        private void FindIntersection(LineSegment line1, LineSegment line2, out bool segmentsIntersect)
        {
            double num1 = line1.Point2.X - line1.Point1.X;
            double num2 = line1.Point2.Y - line1.Point1.Y;
            double num3 = line2.Point2.X - line2.Point1.X;
            double num4 = line2.Point2.Y - line2.Point1.Y;
            double num5 = num2 * num3 - num1 * num4;
            double num6;
            try
            {
                num6 = ((line1.Point1.X - line2.Point1.X) * num4 + (line2.Point1.Y - line1.Point1.Y) * num3) / num5;
            }
            catch
            {
                segmentsIntersect = false;
                return;
            }
            double num7 = ((line2.Point1.X - line1.Point1.X) * num2 + (line1.Point1.Y - line2.Point1.Y) * num1) / -num5;
            segmentsIntersect = num6 >= 0.0 && num6 <= 1.0 && num7 >= 0.0 && num7 <= 1.0;
        }

        private void SeparateHoles(
          List<Polygon> allPolygons,
          List<PolygonOperation.PolygonWithHoles> resultPolygons)
        {
            List<Polygon> polygonList1 = new List<Polygon>();
            List<Polygon> polygonList2 = new List<Polygon>();
            for (int index1 = 0; index1 < allPolygons.Count; ++index1)
            {
                int index2;
                for (index2 = 0; index2 < allPolygons.Count; ++index2)
                {
                    if (index1 != index2)
                    {
                        Polygon polygon1 = new Polygon();
                        polygon1.Points.AddRange((ICollection)allPolygons[index2].Points);
                        Polygon polygon2 = new Polygon();
                        polygon2.Points.AddRange((ICollection)allPolygons[index1].Points);
                        if (this.CsCmpTwoPolygons(polygon1, polygon2) == PolygonOperation.ComparePolygonTypeEnum.POL2_IN_POL1)
                            break;
                    }
                }
                Polygon polygon = new Polygon();
                polygon.Points.AddRange((ICollection)allPolygons[index1].Points);
                if (index2 != allPolygons.Count)
                    polygonList2.Add(polygon);
                else
                    polygonList1.Add(polygon);
            }
            for (int index1 = 0; index1 < polygonList1.Count; ++index1)
            {
                PolygonOperation.PolygonWithHoles polygonWithHoles = new PolygonOperation.PolygonWithHoles();
                polygonWithHoles.contourPolygon = polygonList1[index1];
                for (int index2 = 0; index2 < polygonList2.Count; ++index2)
                {
                    if (this.CsCmpTwoPolygons(polygonList1[index1], polygonList2[index2]) == PolygonOperation.ComparePolygonTypeEnum.POL2_IN_POL1)
                        polygonWithHoles.innerPolygons.Add(polygonList2[index2]);
                }
                resultPolygons.Add(polygonWithHoles);
            }
        }

        public enum ComparePolygonTypeEnum
        {
            POL1_EQ_POL2,
            POL1_COLLIDE_POL2,
            POL1_IN_POL2,
            POL2_IN_POL1,
            POL_OUTSIDE,
        }

        public enum PolygonOperationsEnum
        {
            INTERSECT,
            UNION,
            DIFFERENCE,
        }

        private enum SelectionTypeEnum
        {
            GET_ALL_LINES,
            GET_INTERSECTION_LINES,
        }

        public class CreatePolygons
        {
            private static double offset = 60.0 * Constants.CS_EPSILON;

            public void Create(List<LineSegment> lines, ref List<Polygon> returnPolygons)
            {
                PolygonOperation.CreatePolygons.PolygonLine.GeneratorID = 1;
                returnPolygons.Clear();
                if (lines.Count <= 2)
                    return;
                SetPlane setPlane = new SetPlane(new Tekla.Structures.Model.Model());
                Vector vectorLineSegment1 = Geo.GetVectorLineSegment(lines[0]);
                Vector vectorLineSegment2 = Geo.GetVectorLineSegment(lines[1]);
                Vector normal = vectorLineSegment1.GetNormal();
                Vector newVectorY = vectorLineSegment2.GetNormal();
                bool flag1 = Geo.CompareTwoPoints3D((Point)normal, (Point)newVectorY);
                foreach (LineSegment line in lines)
                {
                    setPlane.AddPoints(line.Point1, line.Point2);
                    if (flag1)
                    {
                        newVectorY = Geo.GetVectorLineSegment(line);
                        newVectorY = newVectorY.GetNormal();
                        flag1 = Geo.CompareTwoPoints3D((Point)normal, (Point)newVectorY);
                    }
                }
                setPlane.Begin(lines[0].Point1, normal, newVectorY);
                try
                {
                    List<PolygonOperation.CreatePolygons.PolygonLine> lines1 = new List<PolygonOperation.CreatePolygons.PolygonLine>();
                    foreach (LineSegment line in lines)
                    {
                        bool flag2 = false;
                        foreach (PolygonOperation.CreatePolygons.PolygonLine polygonLine in lines1)
                        {
                            double beetveenTwoPoints3D1 = Geo.GetDistanceBeetveenTwoPoints3D(line.Point1, polygonLine.Point1);
                            double beetveenTwoPoints3D2 = Geo.GetDistanceBeetveenTwoPoints3D(line.Point2, polygonLine.Point2);
                            if (Compare.LE(beetveenTwoPoints3D1, PolygonOperation.CreatePolygons.offset) && Compare.LE(beetveenTwoPoints3D2, PolygonOperation.CreatePolygons.offset))
                            {
                                flag2 = true;
                                break;
                            }
                            double beetveenTwoPoints3D3 = Geo.GetDistanceBeetveenTwoPoints3D(line.Point1, polygonLine.Point2);
                            double beetveenTwoPoints3D4 = Geo.GetDistanceBeetveenTwoPoints3D(line.Point2, polygonLine.Point1);
                            if (Compare.LE(beetveenTwoPoints3D3, PolygonOperation.CreatePolygons.offset) && Compare.LE(beetveenTwoPoints3D4, PolygonOperation.CreatePolygons.offset))
                            {
                                flag2 = true;
                                break;
                            }
                        }
                        if (!flag2)
                            lines1.Add(new PolygonOperation.CreatePolygons.PolygonLine(line));
                    }
                    List<PolygonOperation.CreatePolygons.PolygonLine> polygonsByLines = new List<PolygonOperation.CreatePolygons.PolygonLine>();
                    List<PolygonOperation.CreatePolygons.PolygonLine> allPolygonLines = new List<PolygonOperation.CreatePolygons.PolygonLine>((IEnumerable<PolygonOperation.CreatePolygons.PolygonLine>)lines1);
                    List<PolygonOperation.CreatePolygons.PolygonLine> polygonLineList1 = new List<PolygonOperation.CreatePolygons.PolygonLine>();
                    while (lines1.Count > 0)
                    {
                        PolygonOperation.CreatePolygons.PolygonLine polygonLine = lines1[0];
                        polygonLine.FoundNeighbours(lines1, true);
                        polygonsByLines.Add(polygonLine);
                        for (int index = lines1.Count - 1; index >= 0; --index)
                        {
                            if (lines1[index].Expanded)
                            {
                                if (lines1[index].Neighbours1.Count == 0 || lines1[index].Neighbours2.Count == 0)
                                    polygonLineList1.Add(lines1[index]);
                                lines1.RemoveAt(index);
                            }
                        }
                    }
                    List<PolygonOperation.CreatePolygons.PolygonLine> polygonLineList2;
                    for (; polygonLineList1.Count > 0; polygonLineList1 = polygonLineList2)
                    {
                        polygonLineList2 = new List<PolygonOperation.CreatePolygons.PolygonLine>();
                        foreach (PolygonOperation.CreatePolygons.PolygonLine polygonLine in polygonLineList1)
                            allPolygonLines.Remove(polygonLine);
                        foreach (PolygonOperation.CreatePolygons.PolygonLine polygonLine1 in allPolygonLines)
                        {
                            foreach (PolygonOperation.CreatePolygons.PolygonLine polygonLine2 in polygonLineList1)
                            {
                                if (polygonLine1.Neighbours1.Contains(polygonLine2))
                                    polygonLine1.Neighbours1.Remove(polygonLine2);
                                if (polygonLine1.Neighbours2.Contains(polygonLine2))
                                    polygonLine1.Neighbours2.Remove(polygonLine2);
                            }
                            if (polygonLine1.Neighbours1.Count == 0 || polygonLine1.Neighbours2.Count == 0)
                                polygonLineList2.Add(polygonLine1);
                        }
                    }
                    List<List<PolygonOperation.CreatePolygons.PolygonLine>> linesPolygons = new List<List<PolygonOperation.CreatePolygons.PolygonLine>>();
                    linesPolygons.Add(new List<PolygonOperation.CreatePolygons.PolygonLine>());
                    this.SetStartLines(allPolygonLines, polygonsByLines);
                    foreach (PolygonOperation.CreatePolygons.PolygonLine polygonLine in polygonsByLines)
                        returnPolygons.Add(polygonLine.GeneratePolygons(linesPolygons, polygonLine.Neighbours1.Count > 0, true));
                    for (int index = returnPolygons.Count - 1; index >= 0; --index)
                    {
                        if (returnPolygons[index].Points.Count == 0)
                        {
                            returnPolygons.RemoveAt(index);
                        }
                        else
                        {
                            foreach (Point point in returnPolygons[index].Points)
                                setPlane.AddPoints(point);
                        }
                    }
                }
                catch
                {
                }
                finally
                {
                    setPlane.End();
                }
            }

            private void SetStartLines(
              List<PolygonOperation.CreatePolygons.PolygonLine> allPolygonLines,
              List<PolygonOperation.CreatePolygons.PolygonLine> polygonsByLines)
            {
                List<List<int>> intListList = new List<List<int>>();
                foreach (PolygonOperation.CreatePolygons.PolygonLine allPolygonLine in allPolygonLines)
                {
                    if (intListList.Count == 0)
                    {
                        List<int> intList = new List<int>();
                        intList.Add(allPolygonLine.ID);
                        intListList.Add(intList);
                        foreach (PolygonOperation.CreatePolygons.PolygonLine polygonLine in allPolygonLine.Neighbours1)
                            intList.Add(polygonLine.ID);
                        foreach (PolygonOperation.CreatePolygons.PolygonLine polygonLine in allPolygonLine.Neighbours2)
                            intList.Add(polygonLine.ID);
                    }
                    else
                    {
                        bool flag = false;
                        foreach (List<int> intList in intListList)
                        {
                            if (intList.Contains(allPolygonLine.ID))
                            {
                                foreach (PolygonOperation.CreatePolygons.PolygonLine polygonLine in allPolygonLine.Neighbours1)
                                {
                                    if (!intList.Contains(polygonLine.ID))
                                        intList.Add(polygonLine.ID);
                                }
                                foreach (PolygonOperation.CreatePolygons.PolygonLine polygonLine in allPolygonLine.Neighbours2)
                                {
                                    if (!intList.Contains(polygonLine.ID))
                                        intList.Add(polygonLine.ID);
                                }
                                flag = true;
                                break;
                            }
                        }
                        if (!flag)
                        {
                            List<int> intList = new List<int>();
                            intList.Add(allPolygonLine.ID);
                            intListList.Add(intList);
                            foreach (PolygonOperation.CreatePolygons.PolygonLine polygonLine in allPolygonLine.Neighbours1)
                                intList.Add(polygonLine.ID);
                            foreach (PolygonOperation.CreatePolygons.PolygonLine polygonLine in allPolygonLine.Neighbours2)
                                intList.Add(polygonLine.ID);
                        }
                    }
                }
                while (intListList.Count > polygonsByLines.Count)
                {
                    for (int index1 = 0; index1 < intListList.Count; ++index1)
                    {
                        for (int index2 = 0; index2 < intListList.Count; ++index2)
                        {
                            if (index1 != index2)
                            {
                                bool flag = false;
                                foreach (int num in intListList[index1])
                                {
                                    if (intListList[index2].Contains(num))
                                    {
                                        flag = true;
                                        break;
                                    }
                                }
                                if (flag)
                                {
                                    foreach (int num in intListList[index2])
                                    {
                                        if (!intListList[index1].Contains(num))
                                            intListList[index1].Add(num);
                                    }
                                    intListList.RemoveAt(index2);
                                    --index1;
                                    break;
                                }
                            }
                        }
                    }
                }
                for (int index = 0; index < polygonsByLines.Count; ++index)
                {
                    foreach (List<int> intList in intListList)
                    {
                        if (intList.Contains(polygonsByLines[index].ID))
                        {
                            int num = int.MaxValue;
                            PolygonOperation.CreatePolygons.PolygonLine polygonLine = polygonsByLines[index];
                            foreach (PolygonOperation.CreatePolygons.PolygonLine allPolygonLine in allPolygonLines)
                            {
                                if (intList.Contains(allPolygonLine.ID) && num > allPolygonLine.Neighbours1.Count + allPolygonLine.Neighbours2.Count)
                                {
                                    num = allPolygonLine.Neighbours1.Count + allPolygonLine.Neighbours2.Count;
                                    polygonLine = allPolygonLine;
                                }
                            }
                            polygonsByLines[index] = polygonLine;
                        }
                    }
                }
            }

            private class PolygonLine : LineSegment
            {
                public static int GeneratorID;

                public PolygonLine(LineSegment line)
                  : base(new Point(line.Point1), new Point(line.Point2))
                {
                    this.Neighbours1 = new List<PolygonOperation.CreatePolygons.PolygonLine>();
                    this.Neighbours2 = new List<PolygonOperation.CreatePolygons.PolygonLine>();
                    this.Expanded = false;
                    this.ID = PolygonOperation.CreatePolygons.PolygonLine.GeneratorID++;
                }

                public bool Expanded { get; set; }

                public int ID { get; private set; }

                public List<PolygonOperation.CreatePolygons.PolygonLine> Neighbours1 { get; set; }

                public List<PolygonOperation.CreatePolygons.PolygonLine> Neighbours2 { get; set; }

                public void FoundNeighbours(
                  List<PolygonOperation.CreatePolygons.PolygonLine> lines,
                  bool first)
                {
                    if (this.Expanded)
                        return;
                    this.Expanded = true;
                    foreach (PolygonOperation.CreatePolygons.PolygonLine line in lines)
                    {
                        if ((LineSegment)line != (LineSegment)this)
                        {
                            double a = Math.Min(Geo.GetDistanceBeetveenTwoPoints3D(line.Point1, this.Point1), Geo.GetDistanceBeetveenTwoPoints3D(line.Point2, this.Point1));
                            double num = Math.Min(Geo.GetDistanceBeetveenTwoPoints3D(line.Point1, this.Point2), Geo.GetDistanceBeetveenTwoPoints3D(line.Point2, this.Point2));
                            if (Compare.LE(a, num))
                            {
                                if (Compare.LE(a, PolygonOperation.CreatePolygons.offset))
                                {
                                    this.Neighbours1.Add(line);
                                    line.FoundNeighbours(lines, false);
                                }
                            }
                            else if (Compare.LE(num, PolygonOperation.CreatePolygons.offset))
                            {
                                this.Neighbours2.Add(line);
                                line.FoundNeighbours(lines, false);
                            }
                        }
                    }
                }

                public Polygon GeneratePolygons(
                  List<List<PolygonOperation.CreatePolygons.PolygonLine>> linesPolygons,
                  bool usePoint1,
                  bool first)
                {
                    Polygon polygon1 = new Polygon();
                    List<PolygonOperation.CreatePolygons.PolygonLine> linesPolygon1 = linesPolygons[linesPolygons.Count - 1];
                    int index1 = this.IsPolygonCompleted(linesPolygon1, usePoint1);
                    if (index1 == -1)
                    {
                        linesPolygon1.Add(this);
                        if (usePoint1)
                        {
                            foreach (PolygonOperation.CreatePolygons.PolygonLine polygonLine in this.Neighbours1)
                                polygonLine.GeneratePolygons(linesPolygons, Geo.GetDistanceBeetveenTwoPoints3D(this.Point1, polygonLine.Point1) > PolygonOperation.CreatePolygons.offset, false);
                        }
                        else
                        {
                            foreach (PolygonOperation.CreatePolygons.PolygonLine polygonLine in this.Neighbours2)
                                polygonLine.GeneratePolygons(linesPolygons, Geo.GetDistanceBeetveenTwoPoints3D(this.Point2, polygonLine.Point1) > PolygonOperation.CreatePolygons.offset, false);
                        }
                        linesPolygon1.RemoveAt(linesPolygon1.Count - 1);
                    }
                    else
                    {
                        linesPolygon1.Add(this);
                        List<PolygonOperation.CreatePolygons.PolygonLine> polygon2 = new List<PolygonOperation.CreatePolygons.PolygonLine>();
                        for (; index1 < linesPolygon1.Count; ++index1)
                            polygon2.Add(linesPolygon1[index1]);
                        if (!this.CheckIfExist(linesPolygons, polygon2))
                            linesPolygons.Insert(0, polygon2);
                        linesPolygon1.Remove(this);
                    }
                    if (first)
                    {
                        linesPolygons.RemoveAt(linesPolygons.Count - 1);
                        double b = double.MinValue;
                        foreach (List<PolygonOperation.CreatePolygons.PolygonLine> linesPolygon2 in linesPolygons)
                        {
                            Polygon polygon2 = new Polygon();
                            for (int index2 = 0; index2 < linesPolygon2.Count; ++index2)
                            {
                                int index3 = index2 + 1;
                                if (index3 >= linesPolygon2.Count)
                                    index3 = 0;
                                if (linesPolygon2[index2].Neighbours1.Contains(linesPolygon2[index3]))
                                    polygon2.Points.Add((object)linesPolygon2[index2].Point1);
                                else
                                    polygon2.Points.Add((object)linesPolygon2[index2].Point2);
                            }
                            double a = PolygonOperation.SummaryPolygonArea2D(polygon2);
                            if (Compare.GE(a, b))
                            {
                                b = a;
                                polygon1 = polygon2;
                            }
                        }
                        linesPolygons.Clear();
                        linesPolygons.Add(new List<PolygonOperation.CreatePolygons.PolygonLine>());
                    }
                    return polygon1;
                }

                private bool CheckIfExist(
                  List<List<PolygonOperation.CreatePolygons.PolygonLine>> linesPolygons,
                  List<PolygonOperation.CreatePolygons.PolygonLine> polygon)
                {
                    bool flag1 = linesPolygons.Count != 1;
                    for (int index = 0; index < linesPolygons.Count - 1; ++index)
                    {
                        flag1 = false;
                        if (linesPolygons[index].Count == polygon.Count)
                        {
                            bool flag2 = true;
                            foreach (PolygonOperation.CreatePolygons.PolygonLine polygonLine in polygon)
                            {
                                flag2 &= linesPolygons[index].Contains(polygonLine);
                                if (!flag2)
                                    break;
                            }
                            if (flag2)
                            {
                                flag1 = true;
                                break;
                            }
                            flag1 = false;
                        }
                    }
                    return flag1;
                }

                private int IsPolygonCompleted(
                  List<PolygonOperation.CreatePolygons.PolygonLine> actualPolygon,
                  bool usePoint1)
                {
                    int num = -1;
                    List<int> intList = new List<int>();
                    if (usePoint1)
                    {
                        for (int index = this.Neighbours1.Count - 1; index >= 0; --index)
                        {
                            num = actualPolygon.IndexOf(this.Neighbours1[index]);
                            if (num != -1)
                                intList.Add(num);
                        }
                    }
                    else
                    {
                        for (int index = this.Neighbours2.Count - 1; index >= 0; --index)
                        {
                            num = actualPolygon.IndexOf(this.Neighbours2[index]);
                            if (num != -1)
                                intList.Add(num);
                        }
                    }
                    if (intList.Count > 0)
                    {
                        intList.Sort();
                        num = intList[intList.Count - 1];
                    }
                    return num;
                }
            }
        }

        public class PolygonWithHoles
        {
            public Polygon contourPolygon;
            public List<Polygon> innerPolygons;

            public PolygonWithHoles()
            {
                this.contourPolygon = new Polygon();
                this.innerPolygons = new List<Polygon>();
            }
        }
    }
}
