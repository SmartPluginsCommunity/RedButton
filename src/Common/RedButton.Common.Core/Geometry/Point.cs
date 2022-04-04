using System;
using RedButton.Common.Core.Geometry.Interfaces;

namespace RedButton.Common.Core.Geometry
{
    /// <summary>
    /// Point 3d class
    /// </summary>
    public class Point : IPoint
    {
        #region Properties

        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }

        #endregion Properties

        #region Constructors

        /// <summary>
        /// Init Zero point
        /// </summary>
        public Point()
        {
            X = 0;
            Y = 0;
            Z = 0;
        }

        /// <summary>
        /// Init X coordinate
        /// </summary>
        /// <param name="x"></param>
        public Point(double x)
        {
            X = x;
            Y = 0;
            Z = 0;
        }

        /// <summary>
        /// Init X, Y coordinates
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public Point(double x, double y)
        {
            X = x;
            Y = y;
            Z = 0;
        }

        /// <summary>
        /// Init X, Y, Z coordinates
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public Point(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public Point(IPoint point)
        {
            this.X = point.X;
            this.Y = point.Y;
            this.Z = point.Z;
        }

        #endregion Constructors

        #region Methods

        public static Point operator+(Point point, Point input)
        {
            return new Point(point.X + input.X, point.Y + input.Y, point.Z + input.Z);
        }
        
        public static Point operator-(Point point, Point input)
        {
            return new Point(point.X -input.X, point.Y - input.Y, point.Z - input.Z);
        }

        public static Point operator +(Point p1, double d)
        {
            return p1 + new Point(d, d, d);
        }
        
        public double Length => X * X + Y * Y + Z * Z;
        
        #endregion Methods
    }
}