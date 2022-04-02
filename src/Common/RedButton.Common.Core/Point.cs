using RedButton.Common.Core.Interfaces;

namespace RedButton.Common.Core
{
    public class Point : IPoint
    {
        #region Properties

        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }

        #endregion Properties

        #region Constructors

        
        public Point()
        {
            X = 0;
            Y = 0;
            Z = 0;
        }

        public Point(double x)
        {
            X = x;
            Y = 0;
            Z = 0;
        }

        public Point(double x, double y)
        {
            X = x;
            Y = y;
            Z = 0;
        }

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
    }
}