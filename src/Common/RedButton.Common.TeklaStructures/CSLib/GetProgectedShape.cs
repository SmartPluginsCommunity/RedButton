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
      var modelObject = new Tekla.Structures.Model.Model().SelectModelObject(partId);
      var flag = false;
      if (modelObject != null && modelObject is Part)
        flag = GetShape(modelObject as Part, ref resultShapePolygons);
      return flag;
    }

    public static bool GetShape(
      Identifier partId,
      CoordinateSystem coordinateSystem,
      ref List<Polygon> resultShapePolygons)
    {
      var modelObject = new Tekla.Structures.Model.Model().SelectModelObject(partId);
      var flag = false;
      if (modelObject != null && modelObject is Part)
        flag = GetShape(modelObject as Part, coordinateSystem, ref resultShapePolygons);
      return flag;
    }

    public static bool GetShape(
      Part part,
      CoordinateSystem coordinateSystem,
      ref List<Polygon> resultShapePolygons)
    {
      var setPlane = new SetPlane(new Tekla.Structures.Model.Model());
      setPlane.Begin(coordinateSystem);
      var flag = GetShape(LineDetermining.GetMainPolygonsFromSolid(part.GetSolid()), ref resultShapePolygons);
      var Plane = new GeometricPlane();
      if (!flag && part is PolyBeam)
      {
        resultShapePolygons.Clear();
        resultShapePolygons.Add(new Polygon());
        foreach (Point Point in part.GetCenterLine(true))
        {
          var plane = Projection.PointToPlane(new Point(Point), Plane);
          resultShapePolygons[0].Points.Add((object) plane);
        }
        flag = true;
      }
      foreach (var polygon in resultShapePolygons)
        setPlane.AddPolygons(polygon);    //хер пойми это одно и тоже или нет что и add из родного cs_net_lib
      setPlane.End();
      return flag;
    }

    public static bool GetShape(Part part, ref List<Polygon> resultShapePolygons)
    {
      var flag = GetShape(LineDetermining.GetMainPolygonsFromSolid(part.GetSolid()), ref resultShapePolygons);
      var Plane = new GeometricPlane();
      if (!flag && part is PolyBeam)
      {
        resultShapePolygons.Clear();
        resultShapePolygons.Add(new Polygon());
        foreach (Point Point in part.GetCenterLine(true))
        {
          var plane = Projection.PointToPlane(new Point(Point), Plane);
          resultShapePolygons[0].Points.Add((object) plane);
        }
        flag = true;
      }
      return flag;
    }

    private static bool GetShape(List<Polygon> polygons, ref List<Polygon> resultShapePolygons)
    {
      resultShapePolygons = new List<Polygon>();
      var polygon = new Polygon();
      var firstLastPoint = new Point();
      var flag = true;
      var lines = LineDetermining.RemoveUselessLines(LineDetermining.GetLineSegmetsFromPolygons(polygons));
      do
      {
        var pointsOfConvexHull = ShapeDetermining.PointsFromConvexHull(lines);
        var linesInConvexHull = ShapeDetermining.LinesInConvexHull(lines, pointsOfConvexHull, ref firstLastPoint);
        if (linesInConvexHull.Count == 0)
        {
          var linesFromPoint = LineDetermining.FindLinesFromPoint(pointsOfConvexHull[0], lines);
          var startLine = ShapeDetermining.FindStartLine(pointsOfConvexHull[0], pointsOfConvexHull[1], linesFromPoint);
          firstLastPoint = ShapeDetermining.FindLastPoint(startLine, pointsOfConvexHull[0]);
          linesInConvexHull.Add(startLine);
        }
        try
        {
          var linesOfShape = ShapeDetermining.ModelBasicShape(lines, linesInConvexHull, firstLastPoint);
          var pointsArrayList = ShapeDetermining.LinesToPointsArrayList(firstLastPoint, linesOfShape);
          polygon.Points = pointsArrayList;
          LineDetermining.DeleteLineFromCreatedPolygon(lines, polygon);
          PolygonOperation.RemoveUnnecessaryPolygonPoints(polygon);
          resultShapePolygons.Add(polygon);
          polygon = new Polygon();
        }
        catch (ShapeNotFoundException ex)
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
        for (var index = lines.Count - 1; index > -1; --index)
        {
          if (Geo.IsPointInsidePolygon2D(resultShapePolygon, lines[index].Point1, true))
            lines.RemoveAt(index);
        }
      }

      public static List<LineSegment> FindLinesFromPoint(
        Point controlPoint,
        List<LineSegment> lines)
      {
        var lineSegmentList = new List<LineSegment>();
        foreach (var line in lines)
        {
          if (Geo.CompareTwoPoints2D(controlPoint, line.Point1) || Geo.CompareTwoPoints2D(controlPoint, line.Point2))
            lineSegmentList.Add(line);
        }
        return lineSegmentList;
      }

      public static List<LineSegment> GetLineSegmetsFromPolygons(
        List<Polygon> polygons)
      {
        var lineSegmentList = new List<LineSegment>();
        foreach (var polygon in polygons)
        {
          var points = polygon.Points;
          for (var index = 0; index < points.Count; ++index)
          {
            if (index < points.Count - 1)
              lineSegmentList.Add(new LineSegment((Point) points[index], (Point) points[index + 1]));
            else
              lineSegmentList.Add(new LineSegment((Point) points[index], (Point) points[0]));
          }
        }
        return lineSegmentList;
      }

      public static List<Polygon> GetMainPolygonsFromSolid(Solid solid)
      {
        var Plane = new GeometricPlane();
        var polygonList = new List<Polygon>();
        var polygon = new Polygon();
        var arrayList = new ArrayList();
        var faceEnumerator = solid.GetFaceEnumerator();
        var num1 = 0;
        var num2 = 0;
        while (faceEnumerator.MoveNext())
        {
          var current1 = faceEnumerator.Current;
          if (current1 != null)
          {
            arrayList.Add((object) current1.Normal);
            var loopEnumerator = current1.GetLoopEnumerator();
            while (loopEnumerator.MoveNext())
            {
              var current2 = loopEnumerator.Current;
              if (current2 != null)
              {
                if (num2 == 0)
                {
                  var vertexEnumerator = current2.GetVertexEnumerator();
                  while (vertexEnumerator.MoveNext())
                  {
                    var current3 = vertexEnumerator.Current;
                    if (current3 != (Point) null)
                    {
                      var plane = Projection.PointToPlane(new Point(current3), Plane);
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
        Solid solid,
        GeometricPlane geoPlane)
      {
        var polygonList = new List<Polygon>();
        var polygon = new Polygon();
        var arrayList = new ArrayList();
        var faceEnumerator = solid.GetFaceEnumerator();
        var num1 = 0;
        var num2 = 0;
        while (faceEnumerator.MoveNext())
        {
          var current1 = faceEnumerator.Current;
          if (current1 != null)
          {
            arrayList.Add((object) current1.Normal);
            var loopEnumerator = current1.GetLoopEnumerator();
            while (loopEnumerator.MoveNext())
            {
              var current2 = loopEnumerator.Current;
              if (current2 != null)
              {
                if (num2 == 0)
                {
                  var vertexEnumerator = current2.GetVertexEnumerator();
                  while (vertexEnumerator.MoveNext())
                  {
                    var current3 = vertexEnumerator.Current;
                    if (current3 != (Point) null)
                    {
                      var plane = Projection.PointToPlane(new Point(current3), geoPlane);
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
        for (var index1 = 0; index1 < lines.Count; ++index1)
        {
          for (var index2 = lines.Count - 1; index2 > index1; --index2)
          {
            if (Geo.CompareTwoLinesSegment2D(lines[index1], lines[index2]))
              lines.RemoveAt(index2);
          }
        }
        for (var index = lines.Count - 1; index > -1; --index)
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
        var otherPoint = FindOtherPoint(mainPoint, lineMain);
        var vector1_1 = new Vector(mainPoint - otherPoint);
        var vector1_2 = new Vector(vector1_1.Y, -vector1_1.X, 0.0);
        var vector2_1 = new Vector(FindOtherPoint(mainPoint, line) - mainPoint);
        var vector2_2 = vector2_1;
        var angleBetween2Vectors1 = GetAngleBetween2Vectors(vector1_2, vector2_2);
        var angleBetween2Vectors2 = GetAngleBetween2Vectors(vector1_1, vector2_1);
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
        var doubleList = new List<double>();
        var otherPoint = FindOtherPoint(mainPoint, lineMain);
        var vector1_1 = new Vector(mainPoint - otherPoint);
        var vector1_2 = new Vector(vector1_1.Y, -vector1_1.X, 0.0);
        foreach (var line in lines)
        {
          var vector2 = new Vector(FindOtherPoint(mainPoint, line) - mainPoint);
          var angleBetween2Vectors1 = GetAngleBetween2Vectors(vector1_2, vector2);
          var angleBetween2Vectors2 = GetAngleBetween2Vectors(vector1_1, vector2);
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
        var angles = FindAngles(new LineSegment(pointConvexHull1, pointConvexHull0), pointConvexHull0, possibleStartLines);
        return FindBestLine(FindBestLines(possibleStartLines, angles));
      }

      public static List<LineSegment> LinesInConvexHull(
        List<LineSegment> lines,
        List<Point> pointsOfConvexHull,
        ref Point firstLastPoint)
      {
        var lineSegmentList1 = new List<LineSegment>();
        var lineSegmentList2 = new List<LineSegment>();
        var flag = false;
        for (var ii = 0; ii < pointsOfConvexHull.Count; ii++)
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
          foreach (var lineSegment in all)
            lineSegmentList1.Add(lineSegment);
        }
        return lineSegmentList1;
      }

      public static List<Point> LinesToPoints(
        List<LineSegment> linesOfShape,
        Point firstLastPoint)
      {
        var pointList = new List<Point>();
        if (Geo.CompareTwoPoints2D(firstLastPoint, linesOfShape[0].Point1))
        {
          var point = new Point(linesOfShape[0].Point1);
          linesOfShape[0].Point1 = linesOfShape[0].Point2;
          linesOfShape[0].Point2 = point;
        }
        foreach (var lineSegment in linesOfShape)
        {
          pointList.Add(lineSegment.Point1);
          pointList.Add(lineSegment.Point2);
        }
        RemoveDuplicitPoints((IList) pointList);
        return pointList;
      }

      public static ArrayList LinesToPointsArrayList(
        Point firstLastPoint,
        List<LineSegment> linesOfShape)
      {
        var arrayList = new ArrayList();
        if (Geo.CompareTwoPoints2D(firstLastPoint, linesOfShape[0].Point1))
        {
          var point = new Point(linesOfShape[0].Point1);
          linesOfShape[0].Point1 = linesOfShape[0].Point2;
          linesOfShape[0].Point2 = point;
        }
        foreach (var lineSegment in linesOfShape)
        {
          arrayList.Add((object) lineSegment.Point1);
          arrayList.Add((object) lineSegment.Point2);
        }
        RemoveDuplicitPoints((IList) arrayList);
        return arrayList;
      }

      public static List<LineSegment> ModelBasicShape(
        List<LineSegment> lines,
        List<LineSegment> linesInConvexHull,
        Point firstLastPoint)
      {
        var lineSegmentList1 = new List<LineSegment>();
        var lineSegment1 = new LineSegment();
        var point = new Point(firstLastPoint);
        var nextLineOfShape = new LineSegment();
        var intersectPoints = new List<Point>();
        var lineSegmentList2 = new List<LineSegment>();
        var intersectLineSegmetsInOneVectorWithPossibleLine = new IntersectLineSegments();
        var num1 = 2;
        var flag = false;
        var num2 = 0;
        lineSegmentList1.Add(linesInConvexHull[0]);
        var nextLine = linesInConvexHull[0];
        linesInConvexHull.RemoveAt(0);
        var otherPoint1 = FindOtherPoint(point, nextLine);
        var lineSegment1_1 = new LineSegment(otherPoint1, firstLastPoint);
        do
        {
          if (FindNextLineInConvex(linesInConvexHull, point, ref nextLine))
          {
            if (Compare.EQ(-180.0, FindAngle(lineSegmentList1[lineSegmentList1.Count - 1], point, nextLine)))
            {
              nextLine = lineSegmentList1[lineSegmentList1.Count - 1];
            }
            else
            {
              lineSegmentList1.Add(nextLine);
              point = FindLastPoint(nextLine, point);
              num1 = 2;
            }
          }
          else if (Intersect.IntersectLineSegmentToLineSegment2D(lineSegment1_1, nextLine, true).Count>0 && num2 > 2)
          {
            var otherPoint2 = FindOtherPoint(point, nextLine);
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
                var nextLines = FindNextLines(point, lines);
                var angles1 = FindAngles(nextLine, point, nextLines);
                lineSegment2 = FindBestLine(FindBestLines(nextLines, angles1));
                break;
              case 1:
                lineSegment2 = nextLineOfShape;
                break;
              default:
                var linesOnEndOfLine = FindPossibleIntersectLinesOnEndOfLine(lines, nextLine, point);
                var angles2 = FindAngles(nextLine, point, linesOnEndOfLine);
                lineSegment2 = FindBestLine(FindBestLines(linesOnEndOfLine, angles2));
                break;
            }
            var possibleIntersectLines = FindPossibleIntersectLines(lines, lineSegment2, point, ref intersectLineSegmetsInOneVectorWithPossibleLine);
            if (possibleIntersectLines.LineSegments.Count > 0)
            {
              do
              {
                var minDistance = FindMinDistance(possibleIntersectLines);
                var lineSegmetsInMin = FindIntersectLineSegmetsInMin(possibleIntersectLines, minDistance);
                var angles3 = FindAngles(new LineSegment(point, lineSegmetsInMin.IntersectPoints[0]), lineSegmetsInMin.IntersectPoints[0], lineSegmetsInMin.LineSegments);
                for (var index = angles3.Count - 1; index > -1; --index)
                {
                  if (Compare.ZR(angles3[index]))
                  {
                    lineSegmetsInMin.IntersectPoints.RemoveAt(index);
                    lineSegmetsInMin.LineSegments.RemoveAt(index);
                    lineSegmetsInMin.Distances.Remove((double) index);
                    angles3.RemoveAt(index);
                  }
                }
                if (FindBestLineAfterIntersect(angles3, lineSegmetsInMin, ref nextLineOfShape))
                {
                  if (!IsItGoodIntersect(lineSegment2, lineSegmetsInMin.IntersectPoints[0], point))
                    lineSegmetsInMin.IntersectPoints[0] = FindOtherPoint(point, lineSegment2);
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
              var angles3 = FindAngles(new LineSegment(point, intersectLineSegmetsInOneVectorWithPossibleLine.IntersectPoints[0]), intersectLineSegmetsInOneVectorWithPossibleLine.IntersectPoints[0], intersectLineSegmetsInOneVectorWithPossibleLine.LineSegments);
              for (var index = angles3.Count - 1; index > -1; --index)
              {
                if (Compare.NZ(angles3[index]))
                {
                  intersectLineSegmetsInOneVectorWithPossibleLine.IntersectPoints.RemoveAt(index);
                  intersectLineSegmetsInOneVectorWithPossibleLine.LineSegments.RemoveAt(index);
                  angles3.RemoveAt(index);
                }
              }
              if (FindBestLineAfterIntersect(angles3, intersectLineSegmetsInOneVectorWithPossibleLine, ref nextLineOfShape))
              {
                num1 = 1;
                var otherPoint2 = FindOtherPoint(intersectLineSegmetsInOneVectorWithPossibleLine.IntersectPoints[0], nextLineOfShape);
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
              point = FindLastPoint(nextLine, point);
            }
            flag = false;
          }
          ++num2;
          if (num2 > 2 * lines.Count)
            throw new ShapeNotFoundException("endless cycle");
        }
        while (!Geo.CompareTwoPoints2D(point, otherPoint1) && (!Geo.CompareTwoPoints2D(point, firstLastPoint) || num2 <= 2));
        return lineSegmentList1;
      }

      public static List<Point> PointsFromConvexHull(List<LineSegment> lines)
      {
        var convexHull = new List<Point>();
        foreach (var line in lines)
        {
          convexHull.Add(line.Point1);
          convexHull.Add(line.Point2);
        }
        RemoveDuplicitPoints((IList) convexHull);
        Geo.ConvexHull(ref convexHull, false);
        return convexHull;
      }

      private static LineSegment FindBestLine(List<LineSegment> bestLines)
      {
        var num = 0.0;
        var index1 = -1;
        for (var index2 = 0; index2 < bestLines.Count; ++index2)
        {
          var beetveenTwoPoints2D = Geo.GetDistanceBeetveenTwoPoints2D(bestLines[index2].Point1, bestLines[index2].Point2);
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
        IntersectLineSegments intersectLineSegmetsInMin,
        ref LineSegment nextLineOfShape)
      {
        var bestLines = new List<LineSegment>();
        nextLineOfShape = new LineSegment();
        var num1 = -1.0;
        var num2 = -1;
        for (var index = 0; index < angles.Count; ++index)
        {
          if (Compare.GT(angles[index], num1))
          {
            num1 = angles[index];
            num2 = index;
          }
        }
        if (Compare.LT(num1, 0.0))
          return false;
        for (var index = num2; index < intersectLineSegmetsInMin.LineSegments.Count; ++index)
        {
          if (Compare.EQ(angles[index], num1))
            bestLines.Add(intersectLineSegmetsInMin.LineSegments[index]);
        }
        nextLineOfShape = FindBestLine(bestLines);
        return true;
      }

      private static List<LineSegment> FindBestLines(
        List<LineSegment> lines,
        List<double> angles)
      {
        var lineSegmentList = new List<LineSegment>();
        var num1 = -181.0;
        var num2 = -1;
        for (var index = 0; index < angles.Count; ++index)
        {
          if (Compare.GT(angles[index], num1))
          {
            num1 = angles[index];
            num2 = index;
          }
        }
        if (Compare.EQ(num1, -181.0))
          throw new ShapeNotFoundException("Cannot find angles in FindBestLine");
        for (var index = num2; index < lines.Count; ++index)
        {
          if (Compare.EQ(angles[index], num1))
            lineSegmentList.Add(lines[index]);
        }
        return lineSegmentList;
      }

      private static IntersectLineSegments FindIntersectLineSegmetsInMin(
        IntersectLineSegments intersectLineSegmets,
        double minDistance)
      {
        var intersectLineSegments = new IntersectLineSegments();
        for (var index = 0; index < intersectLineSegmets.Distances.Count; ++index)
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
        IntersectLineSegments intersectLineSegmets)
      {
        var num = intersectLineSegmets.Distances[0];
        foreach (var distance in intersectLineSegmets.Distances)
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
        foreach (var lineSegment in linesInConvexHull)
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
        var lineSegmentList = new List<LineSegment>();
        for (var index = 0; index < lines.Count; ++index)
        {
          if (Geo.CompareTwoPoints2D(lastPoint, lines[index].Point1) || Geo.CompareTwoPoints2D(lastPoint, lines[index].Point2))
            lineSegmentList.Add(lines[index]);
        }
        return lineSegmentList.Count != 0 ? lineSegmentList : throw new ShapeNotFoundException("Cannot find path on point x: " + lastPoint.X.ToString() + " y: " + lastPoint.Y.ToString());
      }

      private static Point FindOtherPoint(Point mainPoint, LineSegment line) => !Geo.CompareTwoPoints2D(line.Point1, mainPoint) ? line.Point1 : line.Point2;

      private static IntersectLineSegments FindPossibleIntersectLines(
        List<LineSegment> lines,
        LineSegment possibleLineOfShape,
        Point lastPoint,
        ref IntersectLineSegments intersectLineSegmetsInOneVectorWithPossibleLine)
      {
        var intersectPoints = new List<Point>();
        var intersectLineSegments = new IntersectLineSegments();
        intersectLineSegmetsInOneVectorWithPossibleLine = new IntersectLineSegments();
        var otherPoint = FindOtherPoint(lastPoint, possibleLineOfShape);
        var lineSegment = new LineSegment(lastPoint, otherPoint);
        foreach (var line in lines)
        {
          intersectPoints = Intersect.IntersectLineSegmentToLineSegment2D(lineSegment, line, true);
          if (intersectPoints.Count>0)
          {
            if (intersectPoints.Count > 1)
            {
              var angleBetween2Vectors = GetAngleBetween2Vectors(new Vector(lineSegment.Point2 - lineSegment.Point1), new Vector(line.Point2 - line.Point1));
              if (IsIntersectLineSegmetsNotParallel(angleBetween2Vectors))
              {
                if (Geo.CompareTwoPoints2D(intersectPoints[1], lineSegment.Point1) || Geo.CompareTwoPoints2D(intersectPoints[1], lineSegment.Point2) || (Geo.CompareTwoPoints2D(intersectPoints[1], line.Point1) || Geo.CompareTwoPoints2D(intersectPoints[1], line.Point2)))
                  intersectPoints.Remove(intersectPoints[0]);
                else if (Geo.CompareTwoPoints2D(intersectPoints[0], lineSegment.Point1) || Geo.CompareTwoPoints2D(intersectPoints[0], lineSegment.Point2) || (Geo.CompareTwoPoints2D(intersectPoints[0], line.Point1) || Geo.CompareTwoPoints2D(intersectPoints[0], line.Point2)))
                  intersectPoints.Remove(intersectPoints[1]);
              }
              else if (Geo.CompareTwoPoints2D(intersectPoints[1], otherPoint) && !Geo.CompareTwoPoints2D(intersectPoints[1], line.Point1) && !Geo.CompareTwoPoints2D(intersectPoints[1], line.Point2))
              {
                var beetveenTwoPoints2D = Geo.GetDistanceBeetveenTwoPoints2D(intersectPoints[0], lineSegment.Point2);
                var point2 = !Compare.ZR(angleBetween2Vectors) ? line.Point1 : line.Point2;
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
            if (intersectPoints.Count == 1 && IsItLineForIntersect(line, lastPoint) && (IsItLineForIntersect(line, otherPoint) && IsItGoodIntersect(lineSegment, intersectPoints[0], line)))
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
        var intersectPoints = new List<Point>();
        var lineSegmentList = new List<LineSegment>();
        var lineSegment1 = new LineSegment(FindOtherPoint(lastPoint, lastLineOfShape), lastPoint);
        foreach (var line in lines)
        {
          intersectPoints = Intersect.IntersectLineSegmentToLineSegment2D(lineSegment1, line, true);
          if (intersectPoints.Count>0)
          {
            if (!IsItLineForIntersect(line, lastPoint) && intersectPoints.Count == 1)
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
        var num = vector1.GetAngleBetween(vector2) * Constants.RAD_TO_DEG;
        if (Compare.GT(vector1.GetNormal().Cross(vector2.GetNormal()).GetNormal().Y, 0.0))
          num = 360.0 - num;
        return num;
      }

      private static bool IsIntersectLineSegmetsNotParallel(double angle) => Compare.NZ(angle) && Compare.NE(angle, 180.0);

      private static bool IsItGoodIntersect(LineSegment mainLine, Point intersect)
      {
        var flag = true;
        if (Compare.GE(GetAngleBetween2Vectors(new Vector(mainLine.Point2 - mainLine.Point1), new Vector(intersect - mainLine.Point1)), 89.96))
          flag = false;
        return flag;
      }

      private static bool IsItGoodIntersect(LineSegment mainLine, Point intersect, Point lastPoint)
      {
        var flag = true;
        if (Compare.GE(GetAngleBetween2Vectors(new Vector(lastPoint - FindOtherPoint(lastPoint, mainLine)), new Vector(intersect - FindOtherPoint(lastPoint, mainLine))), 90.0))
          flag = false;
        return flag;
      }

      private static bool IsItGoodIntersect(
        LineSegment mainLine,
        Point intersect,
        LineSegment nextPossibleLine)
      {
        var flag = true;
        var vector1_1 = new Vector(mainLine.Point2 - mainLine.Point1);
        var vector2_1 = new Vector(intersect - mainLine.Point1);
        var angleBetween2Vectors1 = GetAngleBetween2Vectors(vector1_1, vector2_1);
        var otherPoint = FindOtherPoint(intersect, nextPossibleLine);
        var vector1_2 = new Vector(vector1_1.Y, -vector1_1.X, 0.0);
        if (Compare.GT(angleBetween2Vectors1, 0.0))
        {
          var vector2_2 = new Vector(otherPoint - intersect);
          var angleBetween2Vectors2 = GetAngleBetween2Vectors(vector1_2, vector2_2);
          var angleBetween2Vectors3 = GetAngleBetween2Vectors(vector1_1, vector2_2);
          var num = !Compare.LE(angleBetween2Vectors2, 90.0) ? angleBetween2Vectors3 : angleBetween2Vectors3 * -1.0;
          if (Compare.LE(num, 0.0) || Compare.GE(num, 90.0))
            flag = false;
        }
        return flag;
      }

      private static bool IsItLineForIntersect(LineSegment line, Point comparisonPoint) => !Geo.CompareTwoPoints2D(comparisonPoint, line.Point1) && !Geo.CompareTwoPoints2D(comparisonPoint, line.Point2);

      private static void RemoveDuplicitPoints(IList points)
      {
        for (var index1 = 0; index1 < points.Count; ++index1)
        {
          for (var index2 = points.Count - 1; index2 > index1; --index2)
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
        IntersectPoints = new List<Point>();
        LineSegments = new List<LineSegment>();
        Distances = new List<double>();
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
