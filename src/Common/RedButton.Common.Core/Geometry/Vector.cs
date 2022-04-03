using RedButton.Common.Core.Geometry.Interfaces;

namespace RedButton.Common.Core.Geometry
{
    public class Vector : IPoint, IVector
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

        public Vector(IPoint input)
        {
            this.X = input.X;
            this.Y = input.Y;
            this.Z = input.Z;
        }
        
        public Vector(IVector input)
        {
            this.X = input.X;
            this.Y = input.Y;
            this.Z = input.Z;
        }

        #endregion Constructors
        
        #region Methods

        

        #endregion Methods
    }
}