using System;
using System.Collections;
using System.Collections.Generic;
using Tekla.Structures.Geometry3d;
using Tekla.Structures.Model;
using Tekla.Structures.Model.Operations;
using Tekla.Structures.Solid;

namespace RedButton.Common.TeklaStructures.CSLib
{
    public class Intersect
    {
        public static List<List<Point>> GetPartFacesPointsWithCuts(Part part)
        {
            Solid solid = part.GetSolid();
            List<List<Point>> pointListList = new List<List<Point>>();
            List<Point> pointList = new List<Point>();
            FaceEnumerator faceEnumerator = solid.GetFaceEnumerator();
            while (faceEnumerator.MoveNext())
            {
                Face current1 = faceEnumerator.Current;
                if (current1 != null)
                {
                    LoopEnumerator loopEnumerator = current1.GetLoopEnumerator();
                    while (loopEnumerator.MoveNext())
                    {
                        Loop current2 = loopEnumerator.Current;
                        if (current2 != null)
                        {
                            VertexEnumerator vertexEnumerator = current2.GetVertexEnumerator();
                            while (vertexEnumerator.MoveNext())
                            {
                                Point current3 = vertexEnumerator.Current;
                                if (current3 != (Point)null)
                                    pointList.Add(current3);
                            }
                            pointListList.Add(new List<Point>((IEnumerable<Point>)pointList));
                            pointList.Clear();
                        }
                    }
                }
            }
            return pointListList;
        }

        public static List<List<Point>> GetPartFacesPointsWithoutCuts(Part part)
        {
            Part part1 = Operation.CopyObject((ModelObject)part, part.GetCoordinateSystem(), part.GetCoordinateSystem()) as Part;
            List<List<Point>> pointListList = new List<List<Point>>();
            if (part1 != null)
            {
                Solid solid = part1.GetSolid();
                List<Point> pointList = new List<Point>();
                FaceEnumerator faceEnumerator = solid.GetFaceEnumerator();
                while (faceEnumerator.MoveNext())
                {
                    Face current1 = faceEnumerator.Current;
                    if (current1 != null)
                    {
                        LoopEnumerator loopEnumerator = current1.GetLoopEnumerator();
                        while (loopEnumerator.MoveNext())
                        {
                            Loop current2 = loopEnumerator.Current;
                            if (current2 != null)
                            {
                                VertexEnumerator vertexEnumerator = current2.GetVertexEnumerator();
                                while (vertexEnumerator.MoveNext())
                                {
                                    Point current3 = vertexEnumerator.Current;
                                    if (current3 != (Point)null)
                                        pointList.Add(current3);
                                }
                                pointListList.Add(new List<Point>((IEnumerable<Point>)pointList));
                                pointList.Clear();
                            }
                        }
                    }
                }
                part1.Delete();
            }
            return pointListList;
        }

        public static ArrayList IntersectAllFaces(
          Solid solid,
          Point point1,
          Point point2,
          Point point3)
        {
            IEnumerator enumerator = solid.IntersectAllFaces(point1, point2, point3);
            ArrayList arrayList = new ArrayList();
            while (enumerator.MoveNext())
                arrayList = enumerator.Current as ArrayList;
            return arrayList;
        }

        public static bool IntersectLineSegmentToLineSegment2D(
          LineSegment lineSegment1,
          LineSegment lineSegment2,
          ref List<Point> intersectPoints)
        {
            LineSegment line = Intersection.LineToLine(new Line(lineSegment1), new Line(lineSegment2));
            intersectPoints.Clear();
            if (line != (LineSegment)null)
            {
                if (Geo.CompareTwoPoints2D(line.Point1, line.Point2))
                {
                    intersectPoints.Add(line.Point1);
                }
                else
                {
                    intersectPoints.Add(line.Point1);
                    intersectPoints.Add(line.Point2);
                }
                for (int index = 0; index < intersectPoints.Count; ++index)
                {
                    if (!Geo.IsPointInLineSegment2D(lineSegment1.Point1, lineSegment1.Point2, intersectPoints[index]) || !Geo.IsPointInLineSegment2D(lineSegment2.Point1, lineSegment2.Point2, intersectPoints[index]))
                    {
                        intersectPoints.RemoveAt(index);
                        --index;
                    }
                }
            }
            else
            {
                if (Geo.IsPointInLineSegment2D(lineSegment1.Point1, lineSegment1.Point2, lineSegment2.Point1))
                    intersectPoints.Add(new Point(lineSegment2.Point1));
                if (Geo.IsPointInLineSegment2D(lineSegment1.Point1, lineSegment1.Point2, lineSegment2.Point2))
                    intersectPoints.Add(new Point(lineSegment2.Point2));
                if (Geo.IsPointInLineSegment2D(lineSegment2.Point1, lineSegment2.Point2, lineSegment1.Point1))
                    intersectPoints.Add(new Point(lineSegment1.Point1));
                if (Geo.IsPointInLineSegment2D(lineSegment2.Point1, lineSegment2.Point2, lineSegment1.Point2))
                    intersectPoints.Add(new Point(lineSegment1.Point2));
                for (int index1 = 0; index1 < intersectPoints.Count; ++index1)
                {
                    for (int index2 = 0; index2 < intersectPoints.Count; ++index2)
                    {
                        if (index1 != index2 && Geo.CompareTwoPoints2D(intersectPoints[index1], intersectPoints[index2]))
                        {
                            intersectPoints.RemoveAt(index2);
                            --index2;
                        }
                    }
                }
                Geo.SortPoints2D(intersectPoints, lineSegment1.Point1);
            }
            return intersectPoints.Count > 0;
        }

        public static bool IntersectLineSegmentToLineSegment2D(
          Point lineSegment1Point1,
          Point lineSegment1Point2,
          Point lineSegment2Point1,
          Point lineSegment2Point2,
          ref List<Point> intersectPoints)
        {
            return IntersectLineSegmentToLineSegment2D(new LineSegment(lineSegment1Point1, lineSegment1Point2), new LineSegment(lineSegment2Point1, lineSegment2Point2), ref intersectPoints);
        }
        
        public static List<Point> IntersectLineSegmentToLineSegment2D(
            LineSegment lineSegment1,
            LineSegment lineSegment2,
            bool usedProjectedShape = false)
        {
            var intersectPoints = new List<Point>();
            if (Geo.CompareTwoLinesSegment2D(lineSegment1, lineSegment2))
            {
                intersectPoints.Add(lineSegment1.Point1);
                intersectPoints.Add(lineSegment1.Point2);
            }
            else
            {
                var line = Intersection.LineToLine(new Line(lineSegment1), new Line(lineSegment2));
                if (line != (LineSegment) null)
                {
                    if (Geo.CompareTwoPoints2D(line.Point1, line.Point2))
                    {
                        intersectPoints.Add(line.Point1);
                    }
                    else
                    {
                        intersectPoints.Add(line.Point1);
                        intersectPoints.Add(line.Point2);
                    }
                    for (int index = 0; index < intersectPoints.Count; ++index)
                    {
                        if (!Geo.IsPointInLineSegment2D(lineSegment1.Point1, lineSegment1.Point2, intersectPoints[index]) || !Geo.IsPointInLineSegment2D(lineSegment2.Point1, lineSegment2.Point2, intersectPoints[index]))
                        {
                            intersectPoints.RemoveAt(index);
                            --index;
                        }
                    }
                }
                else if (!usedProjectedShape)
                    SaveIntersect2DTeklaError(lineSegment1, lineSegment2, ref intersectPoints, usedProjectedShape);
                if (usedProjectedShape)
                    SaveIntersect2DTeklaError(lineSegment1, lineSegment2, ref intersectPoints, usedProjectedShape);
            }
            return intersectPoints;
        }
        
        private static void SaveIntersect2DTeklaError(
            LineSegment line1,
            LineSegment line2,
            ref List<Point> intersectPoints,
            bool usedProjectedShape)
        {
            if (Geo.IsPointInLineSegment2D(line1.Point1, line1.Point2, line2.Point1))
                intersectPoints.Add(new Point(line2.Point1));
            if (Geo.IsPointInLineSegment2D(line1.Point1, line1.Point2, line2.Point2))
                intersectPoints.Add(new Point(line2.Point2));
            if (Geo.IsPointInLineSegment2D(line2.Point1, line2.Point2, line1.Point1))
                intersectPoints.Add(new Point(line1.Point1));
            if (Geo.IsPointInLineSegment2D(line2.Point1, line2.Point2, line1.Point2))
                intersectPoints.Add(new Point(line1.Point2));
            for (int index1 = 0; index1 < intersectPoints.Count; ++index1)
            {
                for (int index2 = index1 + 1; index2 < intersectPoints.Count; ++index2)
                {
                    if (Geo.CompareTwoPoints2D(intersectPoints[index1], intersectPoints[index2]))
                    {
                        intersectPoints.RemoveAt(index2);
                        --index2;
                    }
                }
            }
            Geo.SortPoints2D(intersectPoints, line1.Point1);
            for (int index = 0; usedProjectedShape && index < intersectPoints.Count - 2; ++index)
            {
                if (Geo.IsPointInLineSegment2D(intersectPoints[index], intersectPoints[index + 2], intersectPoints[index + 1]))
                {
                    intersectPoints.RemoveAt(index + 1);
                    --index;
                }
            }
        }


        public static bool IntersectLineSegmentToLineSegment3D(
          LineSegment lineSegment1,
          LineSegment lineSegment2,
          ref List<Point> intersectPoints)
        {
            LineSegment line = Intersection.LineToLine(new Line(lineSegment1), new Line(lineSegment2));
            intersectPoints.Clear();
            if (line != (LineSegment)null)
            {
                if (Geo.CompareTwoPoints3D(line.Point1, line.Point2))
                {
                    intersectPoints.Add(line.Point1);
                }
                else
                {
                    intersectPoints.Add(line.Point1);
                    intersectPoints.Add(line.Point2);
                }
                for (int index = 0; index < intersectPoints.Count; ++index)
                {
                    if (!Geo.IsPointInLineSegment3D(lineSegment1.Point1, lineSegment1.Point2, intersectPoints[index]) || !Geo.IsPointInLineSegment3D(lineSegment2.Point1, lineSegment2.Point2, intersectPoints[index]))
                    {
                        intersectPoints.RemoveAt(index);
                        --index;
                    }
                }
            }
            else
            {
                if (Geo.IsPointInLineSegment3D(lineSegment1.Point1, lineSegment1.Point2, lineSegment2.Point1))
                    intersectPoints.Add(new Point(lineSegment2.Point1));
                if (Geo.IsPointInLineSegment3D(lineSegment1.Point1, lineSegment1.Point2, lineSegment2.Point2))
                    intersectPoints.Add(new Point(lineSegment2.Point2));
                if (Geo.IsPointInLineSegment3D(lineSegment2.Point1, lineSegment2.Point2, lineSegment1.Point1))
                    intersectPoints.Add(new Point(lineSegment1.Point1));
                if (Geo.IsPointInLineSegment3D(lineSegment2.Point1, lineSegment2.Point2, lineSegment1.Point2))
                    intersectPoints.Add(new Point(lineSegment1.Point2));
                for (int index1 = 0; index1 < intersectPoints.Count; ++index1)
                {
                    for (int index2 = 0; index2 < intersectPoints.Count; ++index2)
                    {
                        if (index1 != index2)
                        {
                            if (Geo.CompareTwoPoints3D(intersectPoints[index1], intersectPoints[index2]))
                            {
                                intersectPoints.RemoveAt(index2);
                                --index2;
                            }
                            else if (Compare.LE(Geo.GetDistanceBeetveenTwoPoints3D(intersectPoints[index1], intersectPoints[index2]), 2.0))
                            {
                                intersectPoints.RemoveAt(index2);
                                --index2;
                            }
                        }
                    }
                }
                Geo.SortPoints3D(intersectPoints, lineSegment1.Point1);
            }
            return intersectPoints.Count > 0;
        }

        public static bool IntersectLineSegmentToLineSegment3D(
          Point lineSegment1Point1,
          Point lineSegment1Point2,
          Point lineSegment2Point1,
          Point lineSegment2Point2,
          ref List<Point> intersectPoints)
        {
            return IntersectLineSegmentToLineSegment3D(new LineSegment(lineSegment1Point1, lineSegment1Point2), new LineSegment(lineSegment2Point1, lineSegment2Point2), ref intersectPoints);
        }

        public static bool IntersectLineToCircle2D(
          Point linePoint1,
          Point linePoint2,
          Point centre,
          double radius,
          ref List<Point> intersectPoints)
        {
            intersectPoints.Clear();
            double num1 = linePoint2.X - linePoint1.X;
            double num2 = linePoint2.Y - linePoint1.Y;
            double[] numArray = QuadraticEquation(num1 * num1 + num2 * num2, 2.0 * (num1 * (linePoint1.X - centre.X) + num2 * (linePoint1.Y - centre.Y)), (linePoint1.X - centre.X) * (linePoint1.X - centre.X) + (linePoint1.Y - centre.Y) * (linePoint1.Y - centre.Y) - radius * radius);
            if (!double.IsNaN(numArray[0]))
                intersectPoints.Add(new Point(linePoint1.X + numArray[0] * num1, linePoint1.Y + numArray[0] * num2));
            if (!double.IsNaN(numArray[1]))
                intersectPoints.Add(new Point(linePoint1.X + numArray[1] * num1, linePoint1.Y + numArray[1] * num2));
            return intersectPoints.Count > 0;
        }

        public static bool IntersectLineToLineSegment2D(
          Point linePoint1,
          Point linePoint2,
          Point lineSegmentPoint1,
          Point lineSegmentPoint2,
          ref List<Point> intersectPoints)
        {
            return IntersectLineToLineSegment2D(new Line(linePoint1, linePoint2), new LineSegment(lineSegmentPoint1, lineSegmentPoint2), ref intersectPoints);
        }

        public static bool IntersectLineToLineSegment2D(
          Line line,
          LineSegment lineSegment,
          ref List<Point> intersectPoints)
        {
            Line line2 = new Line(lineSegment);
            LineSegment line1 = Intersection.LineToLine(line, line2);
            intersectPoints.Clear();
            if (line1 != (LineSegment)null)
            {
                if (Geo.CompareTwoPoints2D(line1.Point1, line1.Point2))
                {
                    intersectPoints.Add(line1.Point1);
                }
                else
                {
                    intersectPoints.Add(line1.Point1);
                    intersectPoints.Add(line1.Point2);
                }
                for (int index = 0; index < intersectPoints.Count; ++index)
                {
                    if (!Geo.IsPointInLineSegment2D(lineSegment.Point1, lineSegment.Point2, intersectPoints[index]))
                    {
                        intersectPoints.RemoveAt(index);
                        --index;
                    }
                }
            }
            else
            {
                if (Geo.CompareTwoPoints2D(Projection.PointToLine(lineSegment.Point1, line), lineSegment.Point1))
                    intersectPoints.Add(new Point(lineSegment.Point1));
                if (Geo.CompareTwoPoints2D(Projection.PointToLine(lineSegment.Point2, line), lineSegment.Point2))
                    intersectPoints.Add(new Point(lineSegment.Point2));
            }
            return intersectPoints.Count > 0;
        }

        public static bool IntersectLineToLineSegment3D(
          Point linePoint1,
          Point linePoint2,
          Point lineSegmentPoint1,
          Point lineSegmentPoint2,
          ref List<Point> intersectPoints)
        {
            return IntersectLineToLineSegment3D(new Line(linePoint1, linePoint2), new LineSegment(lineSegmentPoint1, lineSegmentPoint2), ref intersectPoints);
        }

        public static bool IntersectLineToLineSegment3D(
          Line line,
          LineSegment lineSegment,
          ref List<Point> intersectPoints)
        {
            Line line2 = new Line(lineSegment);
            LineSegment line1 = Intersection.LineToLine(line, line2);
            intersectPoints.Clear();
            if (line1 != (LineSegment)null)
            {
                if (Geo.CompareTwoPoints3D(line1.Point1, line1.Point2))
                {
                    intersectPoints.Add(line1.Point1);
                }
                else
                {
                    intersectPoints.Add(line1.Point1);
                    intersectPoints.Add(Geo.GetCenterPoint3D(line1.Point1, line1.Point2));
                    intersectPoints.Add(line1.Point2);
                }
                for (int index = 0; index < intersectPoints.Count; ++index)
                {
                    if (!Geo.IsPointInLineSegment3D(lineSegment.Point1, lineSegment.Point2, intersectPoints[index]))
                    {
                        intersectPoints.RemoveAt(index);
                        --index;
                    }
                    else if (Compare.GT(Geo.GetDistanceBeetveenTwoPoints3D(Projection.PointToLine(intersectPoints[index], line), intersectPoints[index]), 50.0 * Constants.CS_EPSILON))
                    {
                        intersectPoints.RemoveAt(index);
                        --index;
                    }
                }
            }
            else
            {
                if (Geo.CompareTwoPoints3D(Projection.PointToLine(lineSegment.Point1, line), lineSegment.Point1))
                    intersectPoints.Add(new Point(lineSegment.Point1));
                if (Geo.CompareTwoPoints3D(Projection.PointToLine(lineSegment.Point2, line), lineSegment.Point2))
                    intersectPoints.Add(new Point(lineSegment.Point2));
            }
            return intersectPoints.Count > 0;
        }

        private static double[] QuadraticEquation(double operandA, double operandB, double operandC)
        {
            double[] numArray = new double[2];
            double a = operandB * operandB - 4.0 * operandA * operandC;
            if (Compare.LT(a, 0.0))
            {
                numArray[0] = double.NaN;
                numArray[1] = double.NaN;
            }
            else if (Compare.EQ(a, 0.0))
            {
                numArray[0] = (-operandB + a) / (2.0 * operandA);
                numArray[1] = double.NaN;
            }
            else
            {
                numArray[0] = (-operandB + a) / (2.0 * operandA);
                numArray[1] = (-operandB - a) / (2.0 * operandA);
            }
            return numArray;
        }

        public class PlaneSolidIntersect
        {
            private SummaryPlane intersectPlane;
            private Tekla.Structures.Model.Model model;
            private List<SummaryPlane> partPlanes;
            private PolygonOperation po;

            public PlaneSolidIntersect(Tekla.Structures.Model.Model actualModel) => model = actualModel;

            public ArrayList PlaneIntersect(
              Part partToIntersect,
              Point point1,
              Point point2,
              Point point3)
            {
                List<List<Point>> pointListList = PlaneIntersectList(partToIntersect, point1, point2, point3);
                ArrayList arrayList = new ArrayList(pointListList.Count);
                foreach (List<Point> pointList in pointListList)
                {
                    if (pointList.Count > 0)
                        arrayList.Add((object)new ArrayList((ICollection)pointList));
                }
                return arrayList;
            }

            public ArrayList PlaneIntersect(
              List<List<Point>> partToIntersectFaces,
              Point point1,
              Point point2,
              Point point3)
            {
                List<List<Point>> pointListList = PlaneIntersectList(partToIntersectFaces, point1, point2, point3);
                ArrayList arrayList = new ArrayList(pointListList.Count);
                foreach (List<Point> pointList in pointListList)
                {
                    if (pointList.Count > 0)
                        arrayList.Add((object)new ArrayList((ICollection)pointList));
                }
                return arrayList;
            }

            public List<List<Point>> PlaneIntersectList(
              Part partToIntersect,
              Point point1,
              Point point2,
              Point point3)
            {
                return PlaneIntersectList(GetPartFacesPointsWithCuts(partToIntersect), point1, point2, point3);
            }

            public List<List<Point>> PlaneIntersectList(
              List<List<Point>> partToIntersectFaces,
              Point point1,
              Point point2,
              Point point3)
            {
                List<List<List<Point>>> pointListListList = PlaneIntersectPolygonsWithHolesList(partToIntersectFaces, point1, point2, point3);
                List<List<Point>> pointListList = new List<List<Point>>();
                for (int index1 = pointListListList.Count - 1; index1 >= 0; --index1)
                {
                    for (int index2 = 0; index2 < pointListListList[index1].Count; ++index2)
                    {
                        if (index2 == 0)
                            pointListList.Insert(0, pointListListList[index1][index2]);
                        else
                            pointListList.Add(pointListListList[index1][index2]);
                    }
                }
                return pointListList;
            }

            public ArrayList PlaneIntersectPolygonsWithHoles(
              Part partToIntersect,
              Point point1,
              Point point2,
              Point point3)
            {
                List<List<List<Point>>> pointListListList = PlaneIntersectPolygonsWithHolesList(GetPartFacesPointsWithCuts(partToIntersect), point1, point2, point3);
                ArrayList arrayList1 = new ArrayList(pointListListList.Count);
                foreach (List<List<Point>> pointListList in pointListListList)
                {
                    ArrayList arrayList2 = new ArrayList(pointListList.Count);
                    foreach (List<Point> pointList in pointListList)
                    {
                        if (pointList.Count > 0)
                            arrayList2.Add((object)new ArrayList((ICollection)pointList));
                    }
                    arrayList1.Add((object)arrayList2);
                }
                return arrayList1;
            }

            public ArrayList PlaneIntersectPolygonsWithHoles(
              List<List<Point>> partToIntersectFaces,
              Point point1,
              Point point2,
              Point point3)
            {
                List<List<List<Point>>> pointListListList = PlaneIntersectPolygonsWithHolesList(partToIntersectFaces, point1, point2, point3);
                ArrayList arrayList1 = new ArrayList(pointListListList.Count);
                foreach (List<List<Point>> pointListList in pointListListList)
                {
                    ArrayList arrayList2 = new ArrayList(pointListList.Count);
                    foreach (List<Point> pointList in pointListList)
                    {
                        if (pointList.Count > 0)
                            arrayList2.Add((object)new ArrayList((ICollection)pointList));
                    }
                    arrayList1.Add((object)arrayList2);
                }
                return arrayList1;
            }

            public List<List<List<Point>>> PlaneIntersectPolygonsWithHolesList(
              Part partToIntersect,
              Point point1,
              Point point2,
              Point point3)
            {
                return PlaneIntersectPolygonsWithHolesList(GetPartFacesPointsWithCuts(partToIntersect), point1, point2, point3);
            }

            public List<List<List<Point>>> PlaneIntersectPolygonsWithHolesList(
              List<List<Point>> partToIntersectFaces,
              Point point1,
              Point point2,
              Point point3)
            {
                List<List<List<Point>>> pointListListList = new List<List<List<Point>>>();
                SetPlane setPlane = new SetPlane(model);
                setPlane.AddPoints(point1, point2, point3);
                foreach (List<Point> partToIntersectFace in partToIntersectFaces)
                    setPlane.AddPoints(partToIntersectFace.ToArray());
                Vector vectorLineSegment = Geo.GetVectorLineSegment(point1, point2);
                Vector newVectorY = vectorLineSegment.Cross(Geo.GetVectorLineSegment(point1, point3));
                setPlane.Begin(point1, vectorLineSegment, newVectorY);
                try
                {
                    model = new Tekla.Structures.Model.Model();
                    po = new PolygonOperation();
                    partPlanes = new List<SummaryPlane>();
                    intersectPlane = new SummaryPlane(new List<Point>(3)
          {
            new Point(point1),
            new Point(point2),
            new Point(point3)
          }, model);
                    List<List<Point>> faces = new List<List<Point>>(partToIntersectFaces.Count);
                    foreach (List<Point> partToIntersectFace in partToIntersectFaces)
                    {
                        List<Point> pointList = new List<Point>();
                        foreach (Point Point in partToIntersectFace)
                            pointList.Add(new Point(Point));
                        if (pointList.Count > 0)
                            faces.Add(pointList);
                    }
                    RemoveLinesInFaces(faces);
                    CreateSummaryPlanes(faces);
                    LineSolidIntersect lineSolidIntersect = new LineSolidIntersect(model);
                    List<LineSegment> lines = new List<LineSegment>(10);
                    List<List<Point>> pointListList1 = new List<List<Point>>();
                    foreach (SummaryPlane partPlane in partPlanes)
                    {
                        if (!partPlane.IsIntersectFace)
                        {
                            Line line = intersectPlane.IntersectPlane(partPlane);
                            if (line != null)
                                lines.AddRange((IEnumerable<LineSegment>)partPlane.IntersectLine(line));
                        }
                        else
                            pointListList1.Add(new List<Point>((IEnumerable<Point>)partPlane.Polygon));
                    }
                    List<Polygon> polygonList1 = new List<Polygon>(pointListList1.Count);
                    foreach (List<Point> polygonPoints in pointListList1)
                    {
                        polygonList1.Add(Geo.ConvertPolygonFromListPoint(polygonPoints));
                        for (int index1 = 0; index1 < polygonPoints.Count; ++index1)
                        {
                            int index2 = index1 + 1;
                            if (index2 >= polygonPoints.Count)
                                index2 = 0;
                            lines.Add(new LineSegment(new Point(polygonPoints[index1]), new Point(polygonPoints[index2])));
                        }
                    }
                    List<Polygon> returnPolygons = new List<Polygon>();
                    new PolygonOperation.CreatePolygons().Create(lines, ref returnPolygons);
                    try
                    {
                        foreach (Polygon polygon in returnPolygons)
                        {
                            for (int index1 = 0; index1 < polygon.Points.Count; ++index1)
                            {
                                if (polygon.Points.Count > 2)
                                {
                                    int index2 = index1 + 1;
                                    int index3 = index1 + 2;
                                    if (index3 > polygon.Points.Count - 1)
                                        index3 = 0;
                                    if (index2 > polygon.Points.Count - 1)
                                    {
                                        index2 = 0;
                                        index3 = 1;
                                    }
                                    Point point4 = polygon.Points[index1] as Point;
                                    Point point5 = polygon.Points[index2] as Point;
                                    Point point6 = polygon.Points[index3] as Point;
                                    if (Geo.IsPointInLineSegment3D(point4, point6, point5))
                                    {
                                        polygon.Points.RemoveAt(index2);
                                        --index1;
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        //int num = (int)MessageBox.Show(ex.ToString());
                    }
                    returnPolygons.AddRange((IEnumerable<Polygon>)polygonList1);
                    foreach (List<Polygon> polygonList2 in PreparePolygon(returnPolygons))
                    {
                        List<List<Point>> pointListList2 = new List<List<Point>>();
                        foreach (Polygon polygon in polygonList2)
                        {
                            List<Point> pointList = new List<Point>();
                            foreach (Point point in polygon.Points)
                            {
                                pointList.Add(point);
                                setPlane.AddPoints(point);
                            }
                            pointListList2.Add(pointList);
                        }
                        pointListListList.Add(pointListList2);
                    }
                }
                catch
                {
                }
                setPlane.End();
                return pointListListList;
            }

            private void CreateSummaryPlanes(List<List<Point>> faces)
            {
                foreach (List<Point> face in faces)
                    partPlanes.Add(new SummaryPlane(face, model));
                for (int index1 = 0; index1 < partPlanes.Count; ++index1)
                {
                    SetPlane setPlane = new SetPlane(model);
                    setPlane.AddPoints(partPlanes[index1].Polygon.ToArray());
                    for (int index2 = index1 + 1; index2 < partPlanes.Count; ++index2)
                        setPlane.AddPoints(partPlanes[index2].Polygon.ToArray());
                    setPlane.Begin(partPlanes[index1].Polygon[0], partPlanes[index1].VectorT, partPlanes[index1].VectorS);
                    try
                    {
                        for (int index2 = index1 + 1; index2 < partPlanes.Count; ++index2)
                        {
                            bool flag = true;
                            foreach (Point point in partPlanes[index2].Polygon)
                            {
                                if (Compare.NE(point.Z, 0.0))
                                {
                                    flag = false;
                                    break;
                                }
                            }
                            if (flag)
                            {
                                switch (po.CsCmpTwoPolygons(Geo.ConvertPolygonFromListPoint(partPlanes[index1].Polygon), Geo.ConvertPolygonFromListPoint(partPlanes[index2].Polygon)))
                                {
                                    case PolygonOperation.ComparePolygonTypeEnum.POL1_IN_POL2:
                                        partPlanes[index2].SamePlanes.Add(partPlanes[index1]);
                                        partPlanes.RemoveAt(index1);
                                        --index1;
                                        goto label_24;
                                    case PolygonOperation.ComparePolygonTypeEnum.POL2_IN_POL1:
                                        partPlanes[index1].SamePlanes.Add(partPlanes[index2]);
                                        partPlanes.RemoveAt(index2);
                                        --index2;
                                        continue;
                                    default:
                                        continue;
                                }
                            }
                        }
                    }
                    catch
                    {
                    }
                label_24:
                    setPlane.End();
                }
                SetPlane setPlane1 = new SetPlane(model);
                setPlane1.AddPoints(intersectPlane.Polygon.ToArray());
                foreach (SummaryPlane partPlane in partPlanes)
                    setPlane1.AddPoints(partPlane.Polygon.ToArray());
                setPlane1.Begin(intersectPlane.Polygon[0], intersectPlane.VectorT, intersectPlane.VectorS);
                try
                {
                    foreach (SummaryPlane partPlane in partPlanes)
                    {
                        foreach (Point point in partPlane.Polygon)
                        {
                            if (Compare.NE(point.Z, 0.0))
                            {
                                partPlane.IsIntersectFace = false;
                                break;
                            }
                        }
                    }
                }
                catch
                {
                }
                setPlane1.End();
            }

            private List<List<Polygon>> PreparePolygon(List<Polygon> polygons)
            {
                List<List<Polygon>> polygonListList = new List<List<Polygon>>();
                SetPlane setPlane = new SetPlane(model);
                setPlane.AddPolygons(polygons.ToArray());
                setPlane.Begin(intersectPlane.Polygon[0], intersectPlane.VectorS, intersectPlane.VectorT);
                try
                {
                    foreach (Polygon polygon in polygons)
                    {
                        if (polygon.Points.Count == 2 && Geo.CompareTwoPoints3D(polygon.Points[0] as Point, polygon.Points[1] as Point))
                            polygon.Points.RemoveAt(1);
                    }
                label_17:
                    for (int index1 = 0; index1 < polygons.Count; ++index1)
                    {
                        for (int index2 = index1 + 1; index2 < polygons.Count; ++index2)
                        {
                            switch (po.CsCmpTwoPolygons(polygons[index1], polygons[index2]))
                            {
                                case PolygonOperation.ComparePolygonTypeEnum.POL1_EQ_POL2:
                                    polygons.RemoveAt(index2);
                                    --index2;
                                    break;
                                case PolygonOperation.ComparePolygonTypeEnum.POL1_COLLIDE_POL2:
                                    List<PolygonOperation.PolygonWithHoles> polygonWithHolesList = po.PolygonOperations(polygons[index1], polygons[index2], PolygonOperation.PolygonOperationsEnum.UNION);
                                    if (polygonWithHolesList.Count == 1)
                                    {
                                        polygons[index1] = polygonWithHolesList[0].contourPolygon;
                                        polygons.RemoveAt(index2);
                                        --index1;
                                        goto label_17;
                                    }
                                    else
                                        break;
                                case PolygonOperation.ComparePolygonTypeEnum.POL1_IN_POL2:
                                    Polygon polygon = polygons[index1];
                                    polygons[index1] = polygons[index2];
                                    polygons[index2] = polygon;
                                    --index1;
                                    goto label_17;
                            }
                        }
                    }
                    if (polygons.Count > 1)
                    {
                        bool polygonOrientation1 = PolygonOperation.GetPolygonOrientation(polygons[0]);
                        for (int index1 = 0; index1 < polygons.Count; ++index1)
                        {
                            bool flag = true;
                            bool polygonOrientation2 = PolygonOperation.GetPolygonOrientation(polygons[index1]);
                            for (int index2 = 0; index2 < polygons.Count; ++index2)
                            {
                                if (index1 != index2 && po.CsCmpTwoPolygons(polygons[index1], polygons[index2]) == PolygonOperation.ComparePolygonTypeEnum.POL1_IN_POL2)
                                {
                                    flag = false;
                                    break;
                                }
                            }
                            if (flag && polygonOrientation2 != polygonOrientation1)
                                polygons[index1].Points.Reverse();
                            else if (!flag && polygonOrientation2 == polygonOrientation1)
                                polygons[index1].Points.Reverse();
                            if (flag)
                                polygonListList.Add(new List<Polygon>()
                {
                  polygons[index1]
                });
                        }
                        foreach (Polygon polygon in polygons)
                        {
                            foreach (List<Polygon> polygonList in polygonListList)
                            {
                                if (polygon != polygonList[0] && po.CsCmpTwoPolygons(polygon, polygonList[0]) == PolygonOperation.ComparePolygonTypeEnum.POL1_IN_POL2)
                                {
                                    polygonList.Add(polygon);
                                    break;
                                }
                            }
                        }
                    }
                    else if (polygons.Count == 1)
                        polygonListList.Add(new List<Polygon>()
            {
              polygons[0]
            });
                    setPlane.RemoveAllPolygons();
                    setPlane.AddPolygons(polygons.ToArray());
                }
                catch
                {
                }
                setPlane.End();
                return polygonListList;
            }

            private void RemoveLinesInFaces(List<List<Point>> faces)
            {
                foreach (List<Point> face in faces)
                {
                    if (face.Count > 3)
                    {
                        for (int index = 0; index < face.Count - 2; ++index)
                        {
                            double beetveenTwoPoints3D1 = Geo.GetDistanceBeetveenTwoPoints3D(face[index + 1], face[index]);
                            double beetveenTwoPoints3D2 = Geo.GetDistanceBeetveenTwoPoints3D(face[index + 1], face[index + 2]);
                            if (Compare.EQ(Geo.GetDistanceBeetveenTwoPoints3D(face[index + 2], face[index]), beetveenTwoPoints3D1 + beetveenTwoPoints3D2))
                            {
                                face.RemoveAt(index + 1);
                                --index;
                            }
                        }
                    }
                }
            }

            private class SummaryPlane
            {
                private Tekla.Structures.Model.Model model;

                public SummaryPlane(List<Point> facePolygon, Tekla.Structures.Model.Model actualModel)
                {
                    model = actualModel;
                    IsIntersectFace = true;
                    SamePlanes = new List<SummaryPlane>();
                    Polygon = new List<Point>((IEnumerable<Point>)facePolygon);
                    VectorS = Geo.GetVectorLineSegment(Polygon[1], Polygon[0]).GetNormal();
                    VectorT = Geo.GetVectorLineSegment(Polygon[2], Polygon[0]).GetNormal();
                    Normal = new Vector(VectorT.Y * VectorS.Z - VectorT.Z * VectorS.Y, VectorT.Z * VectorS.X - VectorT.X * VectorS.Z, VectorT.X * VectorS.Y - VectorT.Y * VectorS.X);
                }

                public bool IsIntersectFace { get; set; }

                public List<Point> Polygon { get; private set; }

                public List<SummaryPlane> SamePlanes { get; set; }

                public Vector VectorS { get; set; }

                public Vector VectorT { get; set; }

                private Vector Normal { get; set; }

                public List<LineSegment> IntersectLine(Line line)
                {
                    List<Point> points = new List<Point>();
                    List<LineSegment> lineSegmentList1 = new List<LineSegment>();
                    for (int index1 = 0; index1 < Polygon.Count; ++index1)
                    {
                        int index2 = index1 + 1;
                        if (index2 == Polygon.Count)
                            index2 = 0;
                        List<Point> intersectPoints = new List<Point>();
                        IntersectLineToLineSegment3D(line, new LineSegment(Polygon[index1], Polygon[index2]), ref intersectPoints);
                        points.AddRange((IEnumerable<Point>)intersectPoints);
                    }
                    List<Point> pointList = SortPoints(points);
                    if (pointList.Count > 0)
                    {
                        SetPlane setPlane = new SetPlane(model);
                        setPlane.AddPoints(pointList.ToArray());
                        setPlane.AddPoints(Polygon.ToArray());
                        foreach (SummaryPlane samePlane in SamePlanes)
                            setPlane.AddPoints(samePlane.Polygon.ToArray());
                        setPlane.Begin(Polygon[0], VectorT, VectorS);
                        try
                        {
                            if (pointList.Count == 1)
                            {
                                bool flag = true;
                                foreach (SummaryPlane samePlane in SamePlanes)
                                {
                                    if (Geo.IsPointInsidePolygon2D(samePlane.Polygon, pointList[0]))
                                    {
                                        flag = false;
                                        break;
                                    }
                                }
                                if (flag)
                                    lineSegmentList1.Add(new LineSegment(pointList[0], new Point(pointList[0])));
                            }
                            else
                            {
                                for (int index1 = 0; index1 < pointList.Count - 1; ++index1)
                                {
                                    int index2 = index1 + 1;
                                    if (Geo.IsPointInsidePolygon2D(Polygon, Geo.GetCenterPoint2D(pointList[index1], pointList[index2]), true))
                                        lineSegmentList1.Add(new LineSegment(new Point(pointList[index1]), new Point(pointList[index2])));
                                }
                                for (int index1 = 0; index1 < lineSegmentList1.Count - 1; ++index1)
                                {
                                    int index2 = index1 + 1;
                                    if (Geo.CompareTwoPoints3D(lineSegmentList1[index1].Point2, lineSegmentList1[index2].Point1))
                                    {
                                        lineSegmentList1[index1].Point2 = lineSegmentList1[index2].Point2;
                                        lineSegmentList1.RemoveAt(index2);
                                        --index1;
                                    }
                                }
                                foreach (SummaryPlane samePlane in SamePlanes)
                                {
                                    List<LineSegment> lineSegmentList2 = new List<LineSegment>();
                                    foreach (LineSegment lineSegment in lineSegmentList1)
                                        lineSegmentList2.AddRange((IEnumerable<LineSegment>)Geo.CutLineByPolygon2D(lineSegment, samePlane.Polygon, false));
                                    lineSegmentList1 = lineSegmentList2;
                                }
                            }
                            setPlane.RemoveAllPoints();
                            setPlane.AddPoints(Polygon.ToArray());
                            foreach (SummaryPlane samePlane in SamePlanes)
                                setPlane.AddPoints(samePlane.Polygon.ToArray());
                            foreach (LineSegment lineSegment in lineSegmentList1)
                                setPlane.AddPoints(lineSegment.Point1, lineSegment.Point2);
                        }
                        catch
                        {
                        }
                        setPlane.End();
                    }
                    return lineSegmentList1;
                }

                public Line IntersectPlane(SummaryPlane other)
                {
                    Line line = (Line)null;
                    Vector Direction = new Vector(Normal.Y * other.Normal.Z - Normal.Z * other.Normal.Y, Normal.Z * other.Normal.X - Normal.X * other.Normal.Z, Normal.X * other.Normal.Y - Normal.Y * other.Normal.X);
                    Vector vector = new Vector(other.Normal.Y * Direction.Z - other.Normal.Z * Direction.Y, other.Normal.Z * Direction.X - other.Normal.X * Direction.Z, other.Normal.X * Direction.Y - other.Normal.Y * Direction.X);
                    double num1 = Normal.X * vector.X + Normal.Y * vector.Y + Normal.Z * vector.Z;
                    if (Math.Abs(num1) > 0.0)
                    {
                        Vector vectorLineSegment = Geo.GetVectorLineSegment(Polygon[0], other.Polygon[0]);
                        double num2 = (Normal.X * vectorLineSegment.X + Normal.Y * vectorLineSegment.Y + Normal.Z * vectorLineSegment.Z) / num1;
                        line = new Line(other.Polygon[0] + (Point)(num2 * vector), Direction);
                    }
                    return line;
                }

                private List<Point> SortPoints(List<Point> points)
                {
                    List<Point> pointList = new List<Point>();
                    if (points.Count >= 3)
                    {
                        List<List<double>> doubleListList = new List<List<double>>();
                        for (int index = 0; index < points.Count; ++index)
                        {
                            List<double> doubleList = new List<double>();
                            foreach (Point point in points)
                                doubleList.Add(Math.Sqrt((points[index].X - point.X) * (points[index].X - point.X) + (points[index].Y - point.Y) * (points[index].Y - point.Y) + (points[index].Z - point.Z) * (points[index].Z - point.Z)));
                            doubleListList.Add(doubleList);
                        }
                        double num1 = 0.0;
                        int index1 = 0;
                        for (int index2 = 0; index2 < doubleListList.Count; ++index2)
                        {
                            for (int index3 = 0; index3 < doubleListList[index2].Count; ++index3)
                            {
                                if (num1 < doubleListList[index2][index3])
                                {
                                    num1 = doubleListList[index2][index3];
                                    index1 = index3;
                                }
                            }
                        }
                        pointList.Add(new Point(points[index1]));
                        double maxValue = double.MaxValue;
                        double num2 = 0.0;
                        for (int index2 = 1; index2 < points.Count; ++index2)
                        {
                            int index3 = 0;
                            for (int index4 = 0; index4 < points.Count; ++index4)
                            {
                                if (doubleListList[index1][index4] > num2 && doubleListList[index1][index4] < maxValue)
                                {
                                    maxValue = doubleListList[index1][index4];
                                    index3 = index4;
                                }
                            }
                            num2 = maxValue;
                            maxValue = double.MaxValue;
                            pointList.Add(new Point(points[index3]));
                        }
                    }
                    else
                        pointList = points;
                    return pointList;
                }
            }
        }

        public class LineSolidIntersect
        {
            private List<IntersectPointResult> intersectResults;
            private Tekla.Structures.Model.Model model;
            private List<Point> results;

            public LineSolidIntersect(Tekla.Structures.Model.Model actualModel) => model = actualModel;

            public List<Point> LineIntersect(
              List<List<Point>> partFacesPoints,
              Point beginLinePoint,
              Point endLinePoint)
            {
                intersectResults = new List<IntersectPointResult>();
                results = new List<Point>();
                for (int index = 0; index < partFacesPoints.Count; ++index)
                {
                    Point point = IntersectLinePart(partFacesPoints[index][0], partFacesPoints[index][1], partFacesPoints[index][2], beginLinePoint, endLinePoint);
                    if (!double.IsNaN(point.X) && !double.IsNaN(point.Y) && (!double.IsNaN(point.Z) && TestIsPointValid(partFacesPoints[index], point)))
                    {
                        IntersectPointResult intersectPointResult = new IntersectPointResult(point, index);
                        intersectPointResult.IsInLine = IsInLine(partFacesPoints[index], intersectPointResult.Point);
                        intersectResults.Add(intersectPointResult);
                    }
                }
                UniquePoints();
                for (int index = 0; index < intersectResults.Count; ++index)
                {
                    if (intersectResults[index].IsInLine)
                        results.Add(intersectResults[index].Point);
                    else if (intersectResults[index].ParentFaces.Count == 1)
                        results.Add(intersectResults[index].Point);
                }
                SortResults(beginLinePoint);
                return results;
            }

            public List<Point> LineIntersect(
              Part partToIntersect,
              Point beginLinePoint,
              Point endLinePoint)
            {
                return LineIntersect(GetPartFacesPointsWithCuts(partToIntersect), beginLinePoint, endLinePoint);
            }

            public List<Point> LineIntersect(Part partToIntersect, LineSegment lineSegment) => LineIntersect(partToIntersect, lineSegment.Point1, lineSegment.Point2);

            public List<Point> LineIntersect(Part partToIntersect, Line line)
            {
                Vector vector = new Vector((Point)line.Direction);
                vector.Normalize();
                double num = 1000.0;
                Point endLinePoint = new Point(line.Origin.X + vector.X * num, line.Origin.Y + vector.Y * num, line.Origin.Z + vector.Z * num);
                Point beginLinePoint = new Point(line.Origin.X - vector.X * num, line.Origin.Y - vector.Y * num, line.Origin.Z - vector.Z * num);
                return LineIntersect(partToIntersect, beginLinePoint, endLinePoint);
            }

            private Point IntersectLinePart(
              Point planePoint0,
              Point planePoint1,
              Point planePoint2,
              Point startPoint,
              Point endPoint)
            {
                double[] numArray1 = new double[3]
                {
          startPoint.X - endPoint.X,
          startPoint.Y - endPoint.Y,
          startPoint.Z - endPoint.Z
                };
                double[] numArray2 = new double[3]
                {
          planePoint1.X - planePoint0.X,
          planePoint1.Y - planePoint0.Y,
          planePoint1.Z - planePoint0.Z
                };
                double[] numArray3 = new double[3]
                {
          planePoint2.X - planePoint0.X,
          planePoint2.Y - planePoint0.Y,
          planePoint2.Z - planePoint0.Z
                };
                double[] numArray4 = new double[3]
                {
          numArray2[1] * numArray3[2] - numArray2[2] * numArray3[1],
          numArray2[2] * numArray3[0] - numArray2[0] * numArray3[2],
          numArray2[0] * numArray3[1] - numArray2[1] * numArray3[0]
                };
                double num1 = numArray4[0] * planePoint0.X + numArray4[1] * planePoint0.Y + numArray4[2] * planePoint0.Z;
                double num2 = -((numArray4[0] * startPoint.X + numArray4[1] * startPoint.Y + numArray4[2] * startPoint.Z - num1) / (numArray4[0] * numArray1[0] + numArray4[1] * numArray1[1] + numArray4[2] * numArray1[2]));
                return new Point(startPoint.X + numArray1[0] * num2, startPoint.Y + numArray1[1] * num2, startPoint.Z + numArray1[2] * num2);
            }

            private bool IsInLine(List<Point> polygonPoints, Point testPoint)
            {
                for (int index1 = 0; index1 < polygonPoints.Count; ++index1)
                {
                    int index2 = index1 + 1;
                    if (index2 == polygonPoints.Count)
                        index2 = 0;
                    if (Geo.IsPointInLineSegment3D(polygonPoints[index1], polygonPoints[index2], testPoint))
                        return true;
                }
                return false;
            }

            private bool IsNear(ref Point point1, ref Point point2, Point beginLinePoint)
            {
                double beetveenTwoPoints3D = Geo.GetDistanceBeetveenTwoPoints3D(point1, beginLinePoint);
                if (Geo.GetDistanceBeetveenTwoPoints3D(point2, beginLinePoint) >= beetveenTwoPoints3D)
                    return false;
                Point point = point2;
                point2 = point1;
                point1 = point;
                return true;
            }

            private void SortResults(Point beginLinePoint)
            {
                Point[] pointArray = new Point[results.Count];
                for (int index = 0; index < results.Count; ++index)
                    pointArray[index] = results[index];
                bool flag = true;
                while (flag)
                {
                    flag = false;
                    for (int index = 0; index < results.Count; ++index)
                    {
                        if (index + 1 < results.Count && IsNear(ref pointArray[index], ref pointArray[index + 1], beginLinePoint))
                            flag = true;
                    }
                }
                int count = results.Count;
                results.Clear();
                for (int index = 0; index < count; ++index)
                    results.Add(pointArray[index]);
            }

            private bool TestIsPointValid(List<Point> polygonPoints, Point testPoint)
            {
                bool flag = false;
                if (IsInLine(polygonPoints, testPoint))
                    return !flag;
                SetPlane setPlane = new SetPlane(model);
                foreach (Point polygonPoint in polygonPoints)
                    setPlane.AddPoints(polygonPoint);
                setPlane.AddPoints(testPoint);
                Vector vectorLineSegment1 = Geo.GetVectorLineSegment(polygonPoints[1], polygonPoints[0]);
                Vector vectorLineSegment2 = Geo.GetVectorLineSegment(polygonPoints[2], polygonPoints[0]);
                setPlane.Begin(polygonPoints[0], vectorLineSegment1, vectorLineSegment2);
                try
                {
                    flag = Geo.IsPointInsidePolygon2D(polygonPoints, testPoint);
                }
                catch
                {
                }
                setPlane.End();
                return flag;
            }

            private void UniquePoints()
            {
                for (int index1 = 0; index1 < intersectResults.Count; ++index1)
                {
                    for (int index2 = 0; index2 < intersectResults.Count; ++index2)
                    {
                        if (index2 != index1 && Geo.CompareTwoPoints3D(intersectResults[index1].Point, intersectResults[index2].Point))
                        {
                            if (intersectResults[index1].IsInLine || intersectResults[index2].IsInLine)
                                intersectResults[index1].IsInLine = true;
                            intersectResults[index1].ParentFaces.AddRange((IEnumerable<int>)intersectResults[index2].ParentFaces);
                            intersectResults.RemoveAt(index2);
                            --index2;
                        }
                    }
                }
            }

            private class IntersectPointResult
            {
                public IntersectPointResult(Point resultPoint, int parentface)
                {
                    Point = resultPoint;
                    IsInLine = false;
                    ParentFaces = new List<int>();
                    ParentFaces.Add(parentface);
                }

                public bool IsInLine { get; set; }

                public List<int> ParentFaces { get; set; }

                public Point Point { get; set; }
            }
        }

        public class LinearEquationsWithUnknowns
        {
            public LinearEquationsWithUnknowns(int numberOfUnknowns)
            {
                CountOfUnknowns = numberOfUnknowns;
                Data = new double[numberOfUnknowns, numberOfUnknowns + 1];
                Result = new double[numberOfUnknowns];
            }

            public int CountOfUnknowns { get; private set; }

            public double[,] Data { get; set; }

            public double[] Result { get; private set; }

            public void Sum()
            {
                for (int unknown = 0; unknown < CountOfUnknowns - 1; ++unknown)
                    TerminateUnknown(unknown);
                for (int index = CountOfUnknowns - 1; index >= 0; --index)
                {
                    double num = SummaryUnknovn(index, index);
                    Result[index] = (Data[index, CountOfUnknowns] - num) / Data[index, index];
                }
            }

            private void AddLine(int line1, int line2)
            {
                if (line1 == line2)
                    return;
                for (int index = 0; index < CountOfUnknowns + 1; ++index)
                    Data[line1, index] = Data[line1, index] + Data[line2, index];
            }

            private void GetNonZero(int unknown)
            {
                int line2 = unknown;
                int line1 = unknown;
                while (line1 < CountOfUnknowns && Data[line1, unknown] == 0.0)
                    ++line1;
                ChangeLine(line1, line2);
            }

            private void ChangeLine(int line1, int line2)
            {
                if (line1 == CountOfUnknowns || line2 == CountOfUnknowns || line1 == line2)
                    return;
                double[] numArray = new double[CountOfUnknowns + 1];
                for (int index = 0; index < CountOfUnknowns + 1; ++index)
                    numArray[index] = Data[line2, index];
                for (int index = 0; index < CountOfUnknowns + 1; ++index)
                    Data[line2, index] = Data[line1, index];
                for (int index = 0; index < CountOfUnknowns + 1; ++index)
                    Data[line1, index] = numArray[index];
            }

            private void MultiplyLine(int line, double constant)
            {
                for (int index = 0; index < CountOfUnknowns + 1; ++index)
                    Data[line, index] = Data[line, index] * constant;
            }

            private double SummaryUnknovn(int unknovn, int line) => line == CountOfUnknowns - 1 ? 0.0 : SummaryUnknovn(unknovn, line + 1) + Data[unknovn, line + 1] * Result[line + 1];

            private void TerminateUnknown(int unknown)
            {
                GetNonZero(unknown);
                for (int index = unknown + 1; index < CountOfUnknowns; ++index)
                {
                    if (Data[index, unknown] != 0.0)
                    {
                        double constant = Data[unknown, unknown] / -Data[index, unknown];
                        MultiplyLine(index, constant);
                        AddLine(index, unknown);
                    }
                }
            }
        }
    }
}

