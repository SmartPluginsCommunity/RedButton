using System;
using RedButton.Common.Core.Geometry.Interfaces;
using RedButton.Common.Core;
using Math = System.Math;
namespace RedButton.Common.Core.Geometry.Extensions
{
    public static class PointExtension
    {
        /// <summary>
        /// Get center point between two points
        /// </summary>
        /// <param name="point"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public static Point GetCenterPoint(this IPoint point, IPoint input)
        {
            double x = point.X + input.X;
            double y = point.Y + point.Y;
            double z = point.Z + point.Z;
            return new Point(x * 0.5, y * 0.5, z * 0.5);
        }

        /// <summary>
        /// new instance from class
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static Point NewPoint(this IPoint input) => new Point(input);
        
        public static Point NewPoint(this IVector input) => new Point(input.X, input.Y, input.Z);

        public static Point AddVector(this IPoint point, IVector input)
        {
            return new Point(point.X = input.X, point.Y + input.Y, point.Z + input.Z);
        }
        
        /// <summary>
        /// Get distance (double) between 2 points
        /// </summary>
        /// <param name="current"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public static double Distance(this IPoint current, IPoint input)
        {
            double X = current.X;
            double Y = current.Y;
            double Z = current.Z;
            
            return Math.Sqrt(Math.Pow(X - input.X, 2.0) + Math.Pow(Y - input.Y, 2.0) + Math.Pow(Z - input.Z, 2.0));
        }
        
        
        
    }
}