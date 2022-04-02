using RedButton.Common.Core.Interfaces;

namespace RedButton.Common.Core
{
    public class Vector : IPoint
    {
        #region Properties

        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }

        #endregion

        #region Constructors

        
        public Vector()
        {
            X = 0;
            Y = 0;
            Z = 0;
        }

        public Vector(double x)
        {
            X = x;
            Y = 0;
            Z = 0;
        }

        public Vector(double x, double y)
        {
            X = x;
            Y = y;
            Z = 0;
        }

        public Vector(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public Vector(IPoint point)
        {
            this.X = point.X;
            this.Y = point.Y;
            this.Z = point.Z;
        }

        #endregion Constructors
    }
}