using RedButton.Common.Core.Interfaces;

namespace RedButton.Common.Core.Extensions
{
    public static class PointExtension
    {
        public static Point GetCenterPoint(this IPoint point, IPoint input)
        {
            double x = point.X + input.X;
            double y = point.Y + point.Y;
            double z = point.Z + point.Z;
            return new Point(x * 0.5, y * 0.5, z * 0.5);
        }

        public static Point NewPoint(this IPoint input) => new Point(input);
    }
}