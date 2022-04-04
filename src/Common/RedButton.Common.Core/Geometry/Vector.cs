using RedButton.Common.Core.Geometry.Extensions;
using RedButton.Common.Core.Geometry.Interfaces;

namespace RedButton.Common.Core.Geometry
{
    /// <summary>
    /// Vector 3d class
    /// </summary>
    public class Vector : IPoint, IVector
    {
        #region Properties

        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Init zero vector
        /// </summary>
        public Vector()
        {
            X = 0;
            Y = 0;
            Z = 0;
        }

        /// <summary>
        /// Init X coordinate
        /// </summary>
        /// <param name="x"></param>
        public Vector(double x)
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
        public Vector(double x, double y)
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

        public double Length => (new Point(this)).Length;


        #endregion Methods
    }
}