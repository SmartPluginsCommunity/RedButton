using System;
using System.Collections;
using System.Collections.Generic;
using Tekla.Structures;
using Tekla.Structures.Geometry3d;
using Tekla.Structures.Model;
using Tekla.Structures.Solid;

namespace RedButton.Common.TeklaStructures.CSLib
{
  public static class GetProjectedShape
  {
    public static bool GetShape(Identifier partId, ref List<Polygon> resultShapePolygons)
    {
      ModelObject modelObject = new Tekla.Structures.Model.Model().SelectModelObject(partId);
      bool flag = false;
      if (modelObject != null && modelObject is Part)
        flag = GetProjectedShape.GetShape(modelObject as Part, ref resultShapePolygons);
      return flag;
    }

    public static bool GetShape(
      Identifier partId,
      CoordinateSystem coordinateSystem,
      ref List<Polygon> resultShapePolygons)
    {
      ModelObject modelObject = new Tekla.Structures.Model.Model().SelectModelObject(partId);
      bool flag = false;
      if (modelObject != null && modelObject is Part)
        flag = GetProjectedShape.GetShape(modelObject as Part, coordinateSystem, ref resultShapePolygons);
      return flag;
    }

    public static bool GetShape(
      Part part,
      CoordinateSystem coordinateSystem,
      ref List<Polygon> resultShapePolygons)
    {
      SetPlane setPlane = new SetPlane(new Tekla.Structures.Model.Model());
      setPlane.Begin(coordinateSystem);
      bool flag = GetProjectedShape.GetShape(GetProjectedShape.LineDetermining.GetMainPolygonsFromSolid(part.GetSolid()), ref resultShapePolygons);
      GeometricPlane Plane = new GeometricPlane();
      if (!flag && part is PolyBeam)
      {
        resultShapePolygons.Clear();
        resultShapePolygons.Add(new Polygon());
        foreach (Point Point in part.GetCenterLine(true))
        {
          Point plane = Projection.PointToPlane(new Point(Point), Plane);
          resultShapePolygons[0].Points.Add((object) plane);
        }
        flag = true;
      }
      foreach (Polygon polygon in resultShapePolygons)
        setPlane.AddPolygons(polygon);    //хер пойми это одно и тоже или нет что и add из родного cs_net_lib
      setPlane.End();
      return flag;
    }

    public static bool GetShape(Part part, ref List<Polygon> resultShapePolygons)
    {
      bool flag = GetProjectedShape.GetShape(GetProjectedShape.LineDetermining.GetMainPolygonsFromSolid(part.GetSolid()), ref resultShapePolygons);
      GeometricPlane Plane = new GeometricPlane();
      if (!flag && part is PolyBeam)
      {
        resultShapePolygons.Clear();
        resultShapePolygons.Add(new Polygon());
        foreach (Point Point in part.GetCenterLine(true))
        {
          Point plane = Projection.PointToPlane(new Point(Point), Plane);
          resultShapePolygons[0].Points.Add((object) plane);
        }
        flag = true;
      }
      return flag;
    }

    private static bool GetShape(List<Polygon> polygons, ref List<Polygon> resultShapePolygons)
    {
      resultShapePolygons = new List<Polygon>();
      Polygon polygon = new Polygon();
      Point firstLastPoint = new Point();
      bool flag = true;
      List<LineSegment> lines = GetProjectedShape.LineDetermining.RemoveUselessLines(GetProjectedShape.LineDetermining.GetLineSegmetsFromPolygons(polygons));
      do
      {
        List<Point> pointsOfConvexHull = GetProjectedShape.ShapeDetermining.PointsFromConvexHull(lines);
        List<LineSegment> linesInConvexHull = GetProjectedShape.ShapeDetermining.LinesInConvexHull(lines, pointsOfConvexHull, ref firstLastPoint);
        if (linesInConvexHull.Count == 0)
        {
          List<LineSegment> linesFromPoint = GetProjectedShape.LineDetermining.FindLinesFromPoint(pointsOfConvexHull[0], lines);
          LineSegment startLine = GetProjectedShape.ShapeDetermining.FindStartLine(pointsOfConvexHull[0], pointsOfConvexHull[1], linesFromPoint);
          firstLastPoint = GetProjectedShape.ShapeDetermining.FindLastPoint(startLine, pointsOfConvexHull[0]);
          linesInConvexHull.Add(startLine);
        }
        try
        {
          List<LineSegment> linesOfShape = GetProjectedShape.ShapeDetermining.ModelBasicShape(lines, linesInConvexHull, firstLastPoint);
          ArrayList pointsArrayList = GetProjectedShape.ShapeDetermining.LinesToPointsArrayList(firstLastPoint, linesOfShape);
          polygon.Points = pointsArrayList;
          GetProjectedShape.LineDetermining.DeleteLineFromCreatedPolygon(lines, polygon);
          PolygonOperation.RemoveUnnecessaryPolygonPoints(polygon);
          resultShapePolygons.Add(polygon);
          polygon = new Polygon();
        }
        catch (GetProjectedShape.ShapeNotFoundException ex)
        {
          flag = false;
        }
      }
      while ((uint) lines.Count > 0U & flag);
      return flag;
    }

    private static class LineDetermining
    {
      public static void DeleteLineFromCreatedPolygon(
        List<LineSegment> lines,
        Polygon resultShapePolygon)
      {
        for (int index = lines.Count - 1; index > -1; --index)
        {
          if (Geo.IsPointInsidePolygon2D(resultShapePolygon, lines[index].Point1, true))
            lines.RemoveAt(index);
        }
      }

      public static List<LineSegment> FindLinesFromPoint(
        Point controlPoint,
        List<LineSegment> lines)
      {
        List<LineSegment> lineSegmentList = new List<LineSegment>();
        foreach (LineSegment line in lines)
        {
          if (Geo.CompareTwoPoints2D(controlPoint, line.Point1) || Geo.CompareTwoPoints2D(controlPoint, line.Point2))
            lineSegmentList.Add(line);
        }
        return lineSegmentList;
      }

      public static List<LineSegment> GetLineSegmetsFromPolygons(
        List<Polygon> polygons)
      {
        List<LineSegment> lineSegmentList = new List<LineSegment>();
        foreach (Polygon polygon in polygons)
        {
          ArrayList points = polygon.Points;
          for (int index = 0; index < points.Count; ++index)
          {
            if (index < points.Count - 1)
              lineSegmentList.Add(new LineSegment((Point) points[index], (Point) points[index + 1]));
            else
              lineSegmentList.Add(new LineSegment((Point) points[index], (Point) points[0]));
          }
        }
        return lineSegmentList;
      }

      public static List<Polygon> GetMainPolygonsFromSolid(Tekla.Structures.Model.Solid solid)
      {
        GeometricPlane Plane = new GeometricPlane();
        List<Polygon> polygonList = new List<Polygon>();
        Polygon polygon = new Polygon();
        ArrayList arrayList = new ArrayList();
        FaceEnumerator faceEnumerator = solid.GetFaceEnumerator();
        int num1 = 0;
        int num2 = 0;
        while (faceEnumerator.MoveNext())
        {
          Face current1 = faceEnumerator.Current;
          if (current1 != null)
          {
            arrayList.Add((object) current1.Normal);
            LoopEnumerator loopEnumerator = current1.GetLoopEnumerator();
            while (loopEnumerator.MoveNext())
            {
              Loop current2 = loopEnumerator.Current;
              if (current2 != null)
              {
                if (num2 == 0)
                {
                  VertexEnumerator vertexEnumerator = current2.GetVertexEnumerator();
                  while (vertexEnumerator.MoveNext())
                  {
                    Point current3 = vertexEnumerator.Current;
                    if (current3 != (Point) null)
                    {
                      Point plane = Projection.PointToPlane(new Point(current3), Plane);
                      polygon.Points.Add((object) plane);
                    }
                  }
                  polygonList.Add(polygon);
                  polygon = new Polygon();
                }
                ++num2;
                ++num1;
              }
            }
          }
          num2 = 0;
        }
        return polygonList;
      }

      public static List<Polygon> GetMainPolygonsFromSolid(
        Tekla.Structures.Model.Solid solid,
        GeometricPlane geoPlane)
      {
        List<Polygon> polygonList = new List<Polygon>();
        Polygon polygon = new Polygon();
        ArrayList arrayList = new ArrayList();
        FaceEnumerator faceEnumerator = solid.GetFaceEnumerator();
        int num1 = 0;
        int num2 = 0;
        while (faceEnumerator.MoveNext())
        {
          Face current1 = faceEnumerator.Current;
          if (current1 != null)
          {
            arrayList.Add((object) current1.Normal);
            LoopEnumerator loopEnumerator = current1.GetLoopEnumerator();
            while (loopEnumerator.MoveNext())
            {
              Loop current2 = loopEnumerator.Current;
              if (current2 != null)
              {
                if (num2 == 0)
                {
                  VertexEnumerator vertexEnumerator = current2.GetVertexEnumerator();
                  while (vertexEnumerator.MoveNext())
                  {
                    Point current3 = vertexEnumerator.Current;
                    if (current3 != (Point) null)
                    {
                      Point plane = Projection.PointToPlane(new Point(current3), geoPlane);
                      polygon.Points.Add((object) plane);
                    }
                  }
                  polygonList.Add(polygon);
                  polygon = new Polygon();
                }
                ++num2;
                ++num1;
              }
            }
          }
          num2 = 0;
        }
        return polygonList;
      }

      public static List<LineSegment> RemoveUselessLines(List<LineSegment> lines)
      {
        for (int index1 = 0; index1 < lines.Count; ++index1)
        {
          for (int index2 = lines.Count - 1; index2 > index1; --index2)
          {
            if (Geo.CompareTwoLinesSegment2D(lines[index1], lines[index2]))
              lines.RemoveAt(index2);
          }
        }
        for (int index = lines.Count - 1; index > -1; --index)
        {
          if (Geo.CompareTwoPoints2D(lines[index].Point1, lines[index].Point2))
            lines.RemoveAt(index);
        }
        return lines;
      }
    }

    private static class ShapeDetermining
    {
      public static double FindAngle(LineSegment lineMain, Point mainPoint, LineSegment line)
      {
        Point otherPoint = GetProjectedShape.ShapeDetermining.FindOtherPoint(mainPoint, lineMain);
        Vector vector1_1 = new Vector(mainPoint - otherPoint);
        Vector vector1_2 = new Vector(vector1_1.Y, -vector1_1.X, 0.0);
        Vector vector2_1 = new Vector(GetProjectedShape.ShapeDetermining.FindOtherPoint(mainPoint, line) - mainPoint);
        Vector vector2_2 = vector2_1;
        double angleBetween2Vectors1 = GetProjectedShape.ShapeDetermining.GetAngleBetween2Vectors(vector1_2, vector2_2);
        double angleBetween2Vectors2 = GetProjectedShape.ShapeDetermining.GetAngleBetween2Vectors(vector1_1, vector2_1);
        double num;
        if (Compare.LE(angleBetween2Vectors1, 90.0))
        {
          num = angleBetween2Vectors2 * -1.0;
        }
        else
        {
          if (Compare.EQ(angleBetween2Vectors2, 180.0))
            angleBetween2Vectors2 *= -1.0;
          num = angleBetween2Vectors2;
        }
        return num;
      }

      public static List<double> FindAngles(
        LineSegment lineMain,
        Point mainPoint,
        List<LineSegment> lines)
      {
        List<double> doubleList = new List<double>();
        Point otherPoint = GetProjectedShape.ShapeDetermining.FindOtherPoint(mainPoint, lineMain);
        Vector vector1_1 = new Vector(mainPoint - otherPoint);
        Vector vector1_2 = new Vector(vector1_1.Y, -vector1_1.X, 0.0);
        foreach (LineSegment line in lines)
        {
          Vector vector2 = new Vector(GetProjectedShape.ShapeDetermining.FindOtherPoint(mainPoint, line) - mainPoint);
          double angleBetween2Vectors1 = GetProjectedShape.ShapeDetermining.GetAngleBetween2Vectors(vector1_2, vector2);
          double angleBetween2Vectors2 = GetProjectedShape.ShapeDetermining.GetAngleBetween2Vectors(vector1_1, vector2);
          if (Compare.LE(angleBetween2Vectors1, 90.0))
          {
            doubleList.Add(angleBetween2Vectors2 * -1.0);
          }
          else
          {
            if (Compare.EQ(angleBetween2Vectors2, 180.0))
              angleBetween2Vectors2 *= -1.0;
            doubleList.Add(angleBetween2Vectors2);
          }
        }
        return doubleList;
      }

      public static Point FindLastPoint(LineSegment lastLineOfShape, Point lastPoint) => !Geo.CompareTwoPoints2D(lastPoint, lastLineOfShape.Point1) ? lastLineOfShape.Point1 : lastLineOfShape.Point2;

      public static LineSegment FindStartLine(
        Point pointConvexHull0,
        Point pointConvexHull1,
        List<LineSegment> possibleStartLines)
      {
        List<double> angles = GetProjectedShape.ShapeDetermining.FindAngles(new LineSegment(pointConvexHull1, pointConvexHull0), pointConvexHull0, possibleStartLines);
        return GetProjectedShape.ShapeDetermining.FindBestLine(GetProjectedShape.ShapeDetermining.FindBestLines(possibleStartLines, angles));
      }

      public static List<LineSegment> LinesInConvexHull(
        List<LineSegment> lines,
        List<Point> pointsOfConvexHull,
        ref Point firstLastPoint)
      {
        List<LineSegment> lineSegmentList1 = new List<LineSegment>();
        List<LineSegment> lineSegmentList2 = new List<LineSegment>();
        bool flag = false;
        for (int ii = 0; ii < pointsOfConvexHull.Count; ii++)
        {
          List<LineSegment> all;
          if (ii != pointsOfConvexHull.Count - 1)
          {
            all = lines.FindAll((Predicate<LineSegment>) (lineCondition =>
            {
              if (Geo.CompareTwoPoints2D(lineCondition.Point1, pointsOfConvexHull[ii]) && Geo.CompareTwoPoints2D(lineCondition.Point2, pointsOfConvexHull[ii + 1]))
                return true;
              return Geo.CompareTwoPoints2D(lineCondition.Point2, pointsOfConvexHull[ii]) && Geo.CompareTwoPoints2D(lineCondition.Point1, pointsOfConvexHull[ii + 1]);
            }));
            if (!flag && all.Count != 0)
            {
              flag = true;
              firstLastPoint = pointsOfConvexHull[ii + 1];
            }
          }
          else
          {
            all = lines.FindAll((Predicate<LineSegment>) (lineCondition =>
            {
              if (Geo.CompareTwoPoints2D(lineCondition.Point1, pointsOfConvexHull[ii]) && Geo.CompareTwoPoints2D(lineCondition.Point2, pointsOfConvexHull[0]))
                return true;
              return Geo.CompareTwoPoints2D(lineCondition.Point2, pointsOfConvexHull[ii]) && Geo.CompareTwoPoints2D(lineCondition.Point1, pointsOfConvexHull[0]);
            }));
            if (!flag && all.Count != 0)
            {
              flag = true;
              firstLastPoint = pointsOfConvexHull[0];
            }
          }
          foreach (LineSegment lineSegment in all)
            lineSegmentList1.Add(lineSegment);
        }
        return lineSegmentList1;
      }

      public static List<Point> LinesToPoints(
        List<LineSegment> linesOfShape,
        Point firstLastPoint)
      {
        List<Point> pointList = new List<Point>();
        if (Geo.CompareTwoPoints2D(firstLastPoint, linesOfShape[0].Point1))
        {
          Point point = new Point(linesOfShape[0].Point1);
          linesOfShape[0].Point1 = linesOfShape[0].Point2;
          linesOfShape[0].Point2 = point;
        }
        foreach (LineSegment lineSegment in linesOfShape)
        {
          pointList.Add(lineSegment.Point1);
          pointList.Add(lineSegment.Point2);
        }
        GetProjectedShape.ShapeDetermining.RemoveDuplicitPoints((IList) pointList);
        return pointList;
      }

      public static ArrayList LinesToPointsArrayList(
        Point firstLastPoint,
        List<LineSegment> linesOfShape)
      {
        ArrayList arrayList = new ArrayList();
        if (Geo.CompareTwoPoints2D(firstLastPoint, linesOfShape[0].Point1))
        {
          Point point = new Point(linesOfShape[0].Point1);
          linesOfShape[0].Point1 = linesOfShape[0].Point2;
          linesOfShape[0].Point2 = point;
        }
        foreach (LineSegment lineSegment in linesOfShape)
        {
          arrayList.Add((object) lineSegment.Point1);
          arrayList.Add((object) lineSegment.Point2);
        }
        GetProjectedShape.ShapeDetermining.RemoveDuplicitPoints((IList) arrayList);
        return arrayList;
      }

      public static List<LineSegment> ModelBasicShape(
        List<LineSegment> lines,
        List<LineSegment> linesInConvexHull,
        Point firstLastPoint)
      {
        List<LineSegment> lineSegmentList1 = new List<LineSegment>();
        LineSegment lineSegment1 = new LineSegment();
        Point point = new Point(firstLastPoint);
        LineSegment nextLineOfShape = new LineSegment();
        List<Point> intersectPoints = new List<Point>();
        List<LineSegment> lineSegmentList2 = new List<LineSegment>();
        GetProjectedShape.IntersectLineSegments intersectLineSegmetsInOneVectorWithPossibleLine = new GetProjectedShape.IntersectLineSegments();
        int num1 = 2;
        bool flag = false;
        int num2 = 0;
        lineSegmentList1.Add(linesInConvexHull[0]);
        LineSegment nextLine = linesInConvexHull[0];
        linesInConvexHull.RemoveAt(0);
        Point otherPoint1 = GetProjectedShape.ShapeDetermining.FindOtherPoint(point, nextLine);
        LineSegment lineSegment1_1 = new LineSegment(otherPoint1, firstLastPoint);
        do
        {
          if (GetProjectedShape.ShapeDetermining.FindNextLineInConvex(linesInConvexHull, point, ref nextLine))
          {
            if (Compare.EQ(-180.0, GetProjectedShape.ShapeDetermining.FindAngle(lineSegmentList1[lineSegmentList1.Count - 1], point, nextLine)))
            {
              nextLine = lineSegmentList1[lineSegmentList1.Count - 1];
            }
            else
            {
              lineSegmentList1.Add(nextLine);
              point = GetProjectedShape.ShapeDetermining.FindLastPoint(nextLine, point);
              num1 = 2;
            }
          }
          else if (Intersect.IntersectLineSegmentToLineSegment2D(lineSegment1_1, nextLine, ref intersectPoints, true) && num2 > 2)
          {
            Point otherPoint2 = GetProjectedShape.ShapeDetermining.FindOtherPoint(point, nextLine);
            lineSegmentList1[lineSegmentList1.Count - 1].Point2 = otherPoint1;
            lineSegmentList1[lineSegmentList1.Count - 1].Point1 = otherPoint2;
            point = otherPoint1;
          }
          else
          {
            LineSegment lineSegment2;
            switch (num1)
            {
              case 0:
                List<LineSegment> nextLines = GetProjectedShape.ShapeDetermining.FindNextLines(point, lines);
                List<double> angles1 = GetProjectedShape.ShapeDetermining.FindAngles(nextLine, point, nextLines);
                lineSegment2 = GetProjectedShape.ShapeDetermining.FindBestLine(GetProjectedShape.ShapeDetermining.FindBestLines(nextLines, angles1));
                break;
              case 1:
                lineSegment2 = nextLineOfShape;
                break;
              default:
                List<LineSegment> linesOnEndOfLine = GetProjectedShape.ShapeDetermining.FindPossibleIntersectLinesOnEndOfLine(lines, nextLine, point);
                List<double> angles2 = GetProjectedShape.ShapeDetermining.FindAngles(nextLine, point, linesOnEndOfLine);
                lineSegment2 = GetProjectedShape.ShapeDetermining.FindBestLine(GetProjectedShape.ShapeDetermining.FindBestLines(linesOnEndOfLine, angles2));
                break;
            }
            GetProjectedShape.IntersectLineSegments possibleIntersectLines = GetProjectedShape.ShapeDetermining.FindPossibleIntersectLines(lines, lineSegment2, point, ref intersectLineSegmetsInOneVectorWithPossibleLine);
            if (possibleIntersectLines.LineSegments.Count > 0)
            {
              do
              {
                double minDistance = GetProjectedShape.ShapeDetermining.FindMinDistance(possibleIntersectLines);
                GetProjectedShape.IntersectLineSegments lineSegmetsInMin = GetProjectedShape.ShapeDetermining.FindIntersectLineSegmetsInMin(possibleIntersectLines, minDistance);
                List<double> angles3 = GetProjectedShape.ShapeDetermining.FindAngles(new LineSegment(point, lineSegmetsInMin.IntersectPoints[0]), lineSegmetsInMin.IntersectPoints[0], lineSegmetsInMin.LineSegments);
                for (int index = angles3.Count - 1; index > -1; --index)
                {
                  if (Compare.ZR(angles3[index]))
                  {
                    lineSegmetsInMin.IntersectPoints.RemoveAt(index);
                    lineSegmetsInMin.LineSegments.RemoveAt(index);
                    lineSegmetsInMin.Distances.Remove((double) index);
                    angles3.RemoveAt(index);
                  }
                }
                if (GetProjectedShape.ShapeDetermining.FindBestLineAfterIntersect(angles3, lineSegmetsInMin, ref nextLineOfShape))
                {
                  if (!GetProjectedShape.ShapeDetermining.IsItGoodIntersect(lineSegment2, lineSegmetsInMin.IntersectPoints[0], point))
                    lineSegmetsInMin.IntersectPoints[0] = GetProjectedShape.ShapeDetermining.FindOtherPoint(point, lineSegment2);
                  num1 = 1;
                  nextLine = new LineSegment(point, lineSegmetsInMin.IntersectPoints[0]);
                  lineSegmentList1.Add(nextLine);
                  point = lineSegmetsInMin.IntersectPoints[0];
                  flag = true;
                }
              }
              while (possibleIntersectLines.LineSegments.Count > 0 && !flag);
            }
            if (!flag && intersectLineSegmetsInOneVectorWithPossibleLine.LineSegments.Count > 0)
            {
              List<double> angles3 = GetProjectedShape.ShapeDetermining.FindAngles(new LineSegment(point, intersectLineSegmetsInOneVectorWithPossibleLine.IntersectPoints[0]), intersectLineSegmetsInOneVectorWithPossibleLine.IntersectPoints[0], intersectLineSegmetsInOneVectorWithPossibleLine.LineSegments);
              for (int index = angles3.Count - 1; index > -1; --index)
              {
                if (Compare.NZ(angles3[index]))
                {
                  intersectLineSegmetsInOneVectorWithPossibleLine.IntersectPoints.RemoveAt(index);
                  intersectLineSegmetsInOneVectorWithPossibleLine.LineSegments.RemoveAt(index);
                  angles3.RemoveAt(index);
                }
              }
              if (GetProjectedShape.ShapeDetermining.FindBestLineAfterIntersect(angles3, intersectLineSegmetsInOneVectorWithPossibleLine, ref nextLineOfShape))
              {
                num1 = 1;
                Point otherPoint2 = GetProjectedShape.ShapeDetermining.FindOtherPoint(intersectLineSegmetsInOneVectorWithPossibleLine.IntersectPoints[0], nextLineOfShape);
                nextLineOfShape = new LineSegment(point, otherPoint2);
                --num2;
                flag = true;
              }
            }
            if (!flag)
            {
              num1 = 0;
              nextLine = lineSegment2;
              lineSegmentList1.Add(nextLine);
              point = GetProjectedShape.ShapeDetermining.FindLastPoint(nextLine, point);
            }
            flag = false;
          }
          ++num2;
          if (num2 > 2 * lines.Count)
            throw new GetProjectedShape.ShapeNotFoundException("endless cycle");
        }
        while (!Geo.CompareTwoPoints2D(point, otherPoint1) && (!Geo.CompareTwoPoints2D(point, firstLastPoint) || num2 <= 2));
        return lineSegmentList1;
      }

      public static List<Point> PointsFromConvexHull(List<LineSegment> lines)
      {
        List<Point> convexHull = new List<Point>();
        foreach (LineSegment line in lines)
        {
          convexHull.Add(line.Point1);
          convexHull.Add(line.Point2);
        }
        GetProjectedShape.ShapeDetermining.RemoveDuplicitPoints((IList) convexHull);
        Geo.ConvexHull(ref convexHull, false);
        return convexHull;
      }

      private static LineSegment FindBestLine(List<LineSegment> bestLines)
      {
        double num = 0.0;
        int index1 = -1;
        for (int index2 = 0; index2 < bestLines.Count; ++index2)
        {
          double beetveenTwoPoints2D = Geo.GetDistanceBeetveenTwoPoints2D(bestLines[index2].Point1, bestLines[index2].Point2);
          if (Compare.GT(beetveenTwoPoints2D, num))
          {
            num = beetveenTwoPoints2D;
            index1 = index2;
          }
        }
        return bestLines[index1];
      }

      private static bool FindBestLineAfterIntersect(
        List<double> angles,
        GetProjectedShape.IntersectLineSegments intersectLineSegmetsInMin,
        ref LineSegment nextLineOfShape)
      {
        List<LineSegment> bestLines = new List<LineSegment>();
        nextLineOfShape = new LineSegment();
        double num1 = -1.0;
        int num2 = -1;
        for (int index = 0; index < angles.Count; ++index)
        {
          if (Compare.GT(angles[index], num1))
          {
            num1 = angles[index];
            num2 = index;
          }
        }
        if (Compare.LT(num1, 0.0))
          return false;
        for (int index = num2; index < intersectLineSegmetsInMin.LineSegments.Count; ++index)
        {
          if (Compare.EQ(angles[index], num1))
            bestLines.Add(intersectLineSegmetsInMin.LineSegments[index]);
        }
        nextLineOfShape = GetProjectedShape.ShapeDetermining.FindBestLine(bestLines);
        return true;
      }

      private static List<LineSegment> FindBestLines(
        List<LineSegment> lines,
        List<double> angles)
      {
        List<LineSegment> lineSegmentList = new List<LineSegment>();
        double num1 = -181.0;
        int num2 = -1;
        for (int index = 0; index < angles.Count; ++index)
        {
          if (Compare.GT(angles[index], num1))
          {
            num1 = angles[index];
            num2 = index;
          }
        }
        if (Compare.EQ(num1, -181.0))
          throw new GetProjectedShape.ShapeNotFoundException("Cannot find angles in FindBestLine");
        for (int index = num2; index < lines.Count; ++index)
        {
          if (Compare.EQ(angles[index], num1))
            lineSegmentList.Add(lines[index]);
        }
        return lineSegmentList;
      }

      private static GetProjectedShape.IntersectLineSegments FindIntersectLineSegmetsInMin(
        GetProjectedShape.IntersectLineSegments intersectLineSegmets,
        double minDistance)
      {
        GetProjectedShape.IntersectLineSegments intersectLineSegments = new GetProjectedShape.IntersectLineSegments();
        for (int index = 0; index < intersectLineSegmets.Distances.Count; ++index)
        {
          if (Compare.EQ(intersectLineSegmets.Distances[index], minDistance))
          {
            intersectLineSegments.Distances.Add(intersectLineSegmets.Distances[index]);
            intersectLineSegments.IntersectPoints.Add(intersectLineSegmets.IntersectPoints[index]);
            intersectLineSegments.LineSegments.Add(intersectLineSegmets.LineSegments[index]);
            intersectLineSegmets.Distances.RemoveAt(index);
            intersectLineSegmets.IntersectPoints.RemoveAt(index);
            intersectLineSegmets.LineSegments.RemoveAt(index);
            --index;
          }
        }
        return intersectLineSegments;
      }

      private static double FindMinDistance(
        GetProjectedShape.IntersectLineSegments intersectLineSegmets)
      {
        double num = intersectLineSegmets.Distances[0];
        foreach (double distance in intersectLineSegmets.Distances)
        {
          if (Compare.LT(distance, num))
            num = distance;
        }
        return num;
      }

      private static bool FindNextLineInConvex(
        List<LineSegment> linesInConvexHull,
        Point lastPoint,
        ref LineSegment nextLine)
      {
        foreach (LineSegment lineSegment in linesInConvexHull)
        {
          if (Geo.CompareTwoPoints2D(lastPoint, lineSegment.Point1) || Geo.CompareTwoPoints2D(lastPoint, lineSegment.Point2))
          {
            nextLine = lineSegment;
            linesInConvexHull.Remove(lineSegment);
            return true;
          }
        }
        return false;
      }

      private static List<LineSegment> FindNextLines(
        Point lastPoint,
        List<LineSegment> lines)
      {
        List<LineSegment> lineSegmentList = new List<LineSegment>();
        for (int index = 0; index < lines.Count; ++index)
        {
          if (Geo.CompareTwoPoints2D(lastPoint, lines[index].Point1) || Geo.CompareTwoPoints2D(lastPoint, lines[index].Point2))
            lineSegmentList.Add(lines[index]);
        }
        return lineSegmentList.Count != 0 ? lineSegmentList : throw new GetProjectedShape.ShapeNotFoundException("Cannot find path on point x: " + lastPoint.X.ToString() + " y: " + lastPoint.Y.ToString());
      }

      private static Point FindOtherPoint(Point mainPoint, LineSegment line) => !Geo.CompareTwoPoints2D(line.Point1, mainPoint) ? line.Point1 : line.Point2;

      private static GetProjectedShape.IntersectLineSegments FindPossibleIntersectLines(
        List<LineSegment> lines,
        LineSegment possibleLineOfShape,
        Point lastPoint,
        ref GetProjectedShape.IntersectLineSegments intersectLineSegmetsInOneVectorWithPossibleLine)
      {
        List<Point> intersectPoints = new List<Point>();
        GetProjectedShape.IntersectLineSegments intersectLineSegments = new GetProjectedShape.IntersectLineSegments();
        intersectLineSegmetsInOneVectorWithPossibleLine = new GetProjectedShape.IntersectLineSegments();
        Point otherPoint = GetProjectedShape.ShapeDetermining.FindOtherPoint(lastPoint, possibleLineOfShape);
        LineSegment lineSegment = new LineSegment(lastPoint, otherPoint);
        foreach (LineSegment line in lines)
        {
          if (Intersect.IntersectLineSegmentToLineSegment2D(lineSegment, line, ref intersectPoints, true))
          {
            if (intersectPoints.Count > 1)
            {
              double angleBetween2Vectors = GetProjectedShape.ShapeDetermining.GetAngleBetween2Vectors(new Vector(lineSegment.Point2 - lineSegment.Point1), new Vector(line.Point2 - line.Point1));
              if (GetProjectedShape.ShapeDetermining.IsIntersectLineSegmetsNotParallel(angleBetween2Vectors))
              {
                if (Geo.CompareTwoPoints2D(intersectPoints[1], lineSegment.Point1) || Geo.CompareTwoPoints2D(intersectPoints[1], lineSegment.Point2) || (Geo.CompareTwoPoints2D(intersectPoints[1], line.Point1) || Geo.CompareTwoPoints2D(intersectPoints[1], line.Point2)))
                  intersectPoints.Remove(intersectPoints[0]);
                else if (Geo.CompareTwoPoints2D(intersectPoints[0], lineSegment.Point1) || Geo.CompareTwoPoints2D(intersectPoints[0], lineSegment.Point2) || (Geo.CompareTwoPoints2D(intersectPoints[0], line.Point1) || Geo.CompareTwoPoints2D(intersectPoints[0], line.Point2)))
                  intersectPoints.Remove(intersectPoints[1]);
              }
              else if (Geo.CompareTwoPoints2D(intersectPoints[1], otherPoint) && !Geo.CompareTwoPoints2D(intersectPoints[1], line.Point1) && !Geo.CompareTwoPoints2D(intersectPoints[1], line.Point2))
              {
                double beetveenTwoPoints2D = Geo.GetDistanceBeetveenTwoPoints2D(intersectPoints[0], lineSegment.Point2);
                Point point2 = !Compare.ZR(angleBetween2Vectors) ? line.Point1 : line.Point2;
                if (Compare.LT(Geo.GetDistanceBeetveenTwoPoints2D(intersectPoints[0], point2), beetveenTwoPoints2D))
                  intersectPoints[1] = point2;
              }
              if (intersectPoints.Count != 1 && Geo.CompareTwoPoints2D(intersectPoints[1], otherPoint) && (!Geo.CompareTwoPoints2D(intersectPoints[1], line.Point1) && !Geo.CompareTwoPoints2D(intersectPoints[1], line.Point2)))
              {
                intersectLineSegmetsInOneVectorWithPossibleLine.IntersectPoints.Add(otherPoint);
                intersectLineSegmetsInOneVectorWithPossibleLine.LineSegments.Add(new LineSegment(otherPoint, line.Point1));
                intersectLineSegmetsInOneVectorWithPossibleLine.IntersectPoints.Add(otherPoint);
                intersectLineSegmetsInOneVectorWithPossibleLine.LineSegments.Add(new LineSegment(otherPoint, line.Point2));
              }
            }
            if (intersectPoints.Count == 1 && GetProjectedShape.ShapeDetermining.IsItLineForIntersect(line, lastPoint) && (GetProjectedShape.ShapeDetermining.IsItLineForIntersect(line, otherPoint) && GetProjectedShape.ShapeDetermining.IsItGoodIntersect(lineSegment, intersectPoints[0], line)))
            {
              if (Geo.CompareTwoPoints2D(intersectPoints[0], line.Point1) || Geo.CompareTwoPoints2D(intersectPoints[0], line.Point2))
              {
                intersectLineSegments.LineSegments.Add(line);
                intersectLineSegments.IntersectPoints.Add(intersectPoints[0]);
                intersectLineSegments.Distances.Add(Geo.GetDistanceBeetveenTwoPoints2D(lastPoint, intersectPoints[0]));
              }
              else
              {
                intersectLineSegments.LineSegments.Add(new LineSegment(intersectPoints[0], line.Point1));
                intersectLineSegments.IntersectPoints.Add(intersectPoints[0]);
                intersectLineSegments.Distances.Add(Geo.GetDistanceBeetveenTwoPoints2D(lastPoint, intersectPoints[0]));
                intersectLineSegments.LineSegments.Add(new LineSegment(intersectPoints[0], line.Point2));
                intersectLineSegments.IntersectPoints.Add(intersectPoints[0]);
                intersectLineSegments.Distances.Add(Geo.GetDistanceBeetveenTwoPoints2D(lastPoint, intersectPoints[0]));
              }
            }
          }
        }
        return intersectLineSegments;
      }

      private static List<LineSegment> FindPossibleIntersectLinesOnEndOfLine(
        List<LineSegment> lines,
        LineSegment lastLineOfShape,
        Point lastPoint)
      {
        List<Point> intersectPoints = new List<Point>();
        List<LineSegment> lineSegmentList = new List<LineSegment>();
        LineSegment lineSegment1 = new LineSegment(GetProjectedShape.ShapeDetermining.FindOtherPoint(lastPoint, lastLineOfShape), lastPoint);
        foreach (LineSegment line in lines)
        {
          if (Intersect.IntersectLineSegmentToLineSegment2D(lineSegment1, line, ref intersectPoints, true))
          {
            if (!GetProjectedShape.ShapeDetermining.IsItLineForIntersect(line, lastPoint) && intersectPoints.Count == 1)
              lineSegmentList.Add(line);
            else if (intersectPoints.Count > 1 && Geo.CompareTwoPoints2D(intersectPoints[1], lastPoint) && (!Geo.CompareTwoPoints2D(intersectPoints[1], line.Point1) && !Geo.CompareTwoPoints2D(intersectPoints[1], line.Point2)))
            {
              lineSegmentList.Add(new LineSegment(lastPoint, line.Point1));
              lineSegmentList.Add(new LineSegment(lastPoint, line.Point2));
            }
          }
          else if (Geo.CompareTwoPoints2D(lastPoint, line.Point1) && !Geo.CompareTwoPoints2D(lastPoint, line.Point2) || Geo.CompareTwoPoints2D(lastPoint, line.Point2) && !Geo.CompareTwoPoints2D(lastPoint, line.Point1))
            lineSegmentList.Add(line);
        }
        return lineSegmentList;
      }

      private static double GetAngleBetween2Vectors(Vector vector1, Vector vector2)
      {
        double num = vector1.GetAngleBetween(vector2) * Constants.RAD_TO_DEG;
        if (Compare.GT(vector1.GetNormal().Cross(vector2.GetNormal()).GetNormal().Y, 0.0))
          num = 360.0 - num;
        return num;
      }

      private static bool IsIntersectLineSegmetsNotParallel(double angle) => Compare.NZ(angle) && Compare.NE(angle, 180.0);

      private static bool IsItGoodIntersect(LineSegment mainLine, Point intersect)
      {
        bool flag = true;
        if (Compare.GE(GetProjectedShape.ShapeDetermining.GetAngleBetween2Vectors(new Vector(mainLine.Point2 - mainLine.Point1), new Vector(intersect - mainLine.Point1)), 89.96))
          flag = false;
        return flag;
      }

      private static bool IsItGoodIntersect(LineSegment mainLine, Point intersect, Point lastPoint)
      {
        bool flag = true;
        if (Compare.GE(GetProjectedShape.ShapeDetermining.GetAngleBetween2Vectors(new Vector(lastPoint - GetProjectedShape.ShapeDetermining.FindOtherPoint(lastPoint, mainLine)), new Vector(intersect - GetProjectedShape.ShapeDetermining.FindOtherPoint(lastPoint, mainLine))), 90.0))
          flag = false;
        return flag;
      }

      private static bool IsItGoodIntersect(
        LineSegment mainLine,
        Point intersect,
        LineSegment nextPossibleLine)
      {
        bool flag = true;
        Vector vector1_1 = new Vector(mainLine.Point2 - mainLine.Point1);
        Vector vector2_1 = new Vector(intersect - mainLine.Point1);
        double angleBetween2Vectors1 = GetProjectedShape.ShapeDetermining.GetAngleBetween2Vectors(vector1_1, vector2_1);
        Point otherPoint = GetProjectedShape.ShapeDetermining.FindOtherPoint(intersect, nextPossibleLine);
        Vector vector1_2 = new Vector(vector1_1.Y, -vector1_1.X, 0.0);
        if (Compare.GT(angleBetween2Vectors1, 0.0))
        {
          Vector vector2_2 = new Vector(otherPoint - intersect);
          double angleBetween2Vectors2 = GetProjectedShape.ShapeDetermining.GetAngleBetween2Vectors(vector1_2, vector2_2);
          double angleBetween2Vectors3 = GetProjectedShape.ShapeDetermining.GetAngleBetween2Vectors(vector1_1, vector2_2);
          double num = !Compare.LE(angleBetween2Vectors2, 90.0) ? angleBetween2Vectors3 : angleBetween2Vectors3 * -1.0;
          if (Compare.LE(num, 0.0) || Compare.GE(num, 90.0))
            flag = false;
        }
        return flag;
      }

      private static bool IsItLineForIntersect(LineSegment line, Point comparisonPoint) => !Geo.CompareTwoPoints2D(comparisonPoint, line.Point1) && !Geo.CompareTwoPoints2D(comparisonPoint, line.Point2);

      private static void RemoveDuplicitPoints(IList points)
      {
        for (int index1 = 0; index1 < points.Count; ++index1)
        {
          for (int index2 = points.Count - 1; index2 > index1; --index2)
          {
            if (Geo.CompareTwoPoints2D((Point) points[index1], (Point) points[index2]))
              points.RemoveAt(index2);
          }
        }
      }
    }

    private class IntersectLineSegments
    {
      public IntersectLineSegments()
      {
        this.IntersectPoints = new List<Point>();
        this.LineSegments = new List<LineSegment>();
        this.Distances = new List<double>();
      }

      public List<double> Distances { get; set; }

      public List<Point> IntersectPoints { get; set; }

      public List<LineSegment> LineSegments { get; set; }
    }

    private class ShapeNotFoundException : Exception
    {
      public ShapeNotFoundException(string message)
        : base(message)
      {
      }
    }
  }
}
