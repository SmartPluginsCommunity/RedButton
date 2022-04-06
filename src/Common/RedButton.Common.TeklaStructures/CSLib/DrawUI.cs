using Tekla.Structures.Geometry3d;
using Tekla.Structures.Model.UI;

namespace RedButton.Common.TeklaStructures.CSLib
{
    public static class DrawUI
    {
        public static Color white = new Color(1.0, 1.0, 1.0);
        public static Color black = new Color(0.0, 0.0, 0.0);
        public static Color blue = new Color(0.0, 0.0, 1.0);
        public static Color red = new Color(1.0, 0.0, 0.0);
        public static Color green = new Color(0.0, 1.0, 0.0);
        public static Color purple = new Color(1.0, 0.0, 1.0);

        public static void DrawPlane(CoordinateSystem planeCoord, Color lineColor, Color textColor, string comment)
        {
            Vector vector = planeCoord.AxisX.Cross(planeCoord.AxisY);
            planeCoord.AxisX.Normalize(1500.0);
            planeCoord.AxisY.Normalize(1500.0);
            vector.Normalize(1500.0);
            GraphicsDrawer graphicsDrawer = new GraphicsDrawer();
            Point point1 = new Point(planeCoord.Origin + (Point)planeCoord.AxisX);
            Point point2 = new Point(planeCoord.Origin + (Point)planeCoord.AxisY);
            Point point3 = new Point(planeCoord.Origin + (Point)vector);
            graphicsDrawer.DrawLineSegment(planeCoord.Origin, point1, lineColor);
            graphicsDrawer.DrawLineSegment(planeCoord.Origin, point2, lineColor);
            graphicsDrawer.DrawLineSegment(planeCoord.Origin, point3, lineColor);
            graphicsDrawer.DrawText(point1, "X", textColor);
            graphicsDrawer.DrawText(point2, "Y", textColor);
            graphicsDrawer.DrawText(point3, "Z", textColor);
            graphicsDrawer.DrawText(planeCoord.Origin, comment, textColor);
        }

        public static void DrawPlane(Color lineColor, Color textColor) => DrawPlane(new CoordinateSystem(), lineColor, textColor, "");

        public static void DrawPlane(string comment) => DrawPlane(new CoordinateSystem(), red, blue, comment);

        public static void DrawPlane(CoordinateSystem planeCoord) => DrawPlane(planeCoord, red, blue, "");

        public static void DrawPlane() => DrawPlane(new CoordinateSystem(), red, blue, "");

        public static void DrawLine(Point p1, Point p2, string comment1, string comment2)
        {
            DrawPoint(p1, comment1);
            DrawPoint(p2, comment2);
            DrawLine(p1, p2, red);
        }

        public static void DrawLine(Point p1, Point p2, Color textColor) => new GraphicsDrawer().DrawLineSegment(p1, p2, textColor);

        public static void DrawLine(Point p1, Point p2) => DrawLine(p1, p2, red);

        public static void DrawPoint(Point point, string text) => DrawPoint(point, text, red);

        public static void DrawPoint(Point point, string text, Color textColor) => new GraphicsDrawer().DrawText(point, "..." + text, textColor);
    }
}