using RedButton.Common.Core.Geometry.Interfaces;
using RedButton.Common.Core;
namespace RedButton.Common.Core.Geometry.Extensions
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
        
        public static double Distance(this IPoint current, IPoint input)
        {
            double X = current.X;
            double Y = current.Y;
            double Z = current.Z;
            
            return Math.Math.Sqrt(Math.Math.Pow(X - input.X, 2.0) + Math.Math.Pow(Y - input.Y, 2.0) + Math.Math.Pow(Z - input.Z, 2.0));
        }
        
    }
}