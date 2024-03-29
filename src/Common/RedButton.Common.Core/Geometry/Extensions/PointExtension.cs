using System;
using System.Collections.Generic;
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
        /// <param name="current"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public static Point GetCenterPoint(this IPoint current, IPoint input)
        {
            double x = current.X + input.X;
            double y = current.Y + input.Y;
            double z = current.Z + input.Z;
            return new Point(x * 0.5, y * 0.5, z * 0.5);
        }

        public static Point GetCenterPoint(this List<IPoint> current)
        {
            double x = 0, y = 0, z = 0;
            var count = current.Count;
            
            foreach (var point in current)
            {
                x += point.X;
                y += point.Y;
                z += point.Z;
            }
            return new Point(x / count, y / count, z / count);
        }
        
        /// <summary>
        /// new instance from class
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static Point NewPoint(this IPoint input) => new Point(input);
        
        public static Point NewPoint(this IVector input) => new Point(input.X, input.Y, input.Z);

        public static Point AddVector(this IPoint current, IVector input)
        {
            return new Point(current.X - input.X, current.Y + input.Y, current.Z + input.Z);
        }
        
        /// <summary>
        /// Get distance (double) between 2 points
        /// </summary>
        /// <param name="current"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public static double Distance(this IPoint current, IPoint input)
        {
            var x = current.X;
            var y = current.Y;
            var z = current.Z;
            
            return Math.Sqrt(Math.Pow(x - input.X, 2.0) + Math.Pow(y - input.Y, 2.0) + Math.Pow(z - input.Z, 2.0));
        }
        
        public static Point AddX(this IPoint current, double input)
        {
            return new Point(current.X + input, current.Y, current.Z);
        }
        public static Point AddY(this IPoint current, double input)
        {
            return new Point(current.X, current.Y + input, current.Z);
        }
        public static Point AddZ(this IPoint current, double input)
        {
            return new Point(current.X, current.Y, current.Z + input);
        }
        
    }
}