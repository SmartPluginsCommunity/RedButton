using System;
using System.Runtime.InteropServices;
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
        
        public double Length => Math.Sqrt(ScalarProduct(this));

        #region Operators

        public static Vector operator +(Vector current, Vector input)
        {
            return new Vector(current.X + input.X, current.Y + input.Y, current.Z + input.Z);
        }
        public static Vector operator -(Vector current, Vector input)
        {
            return new Vector(current.X - input.X, current.Y - input.Y, current.Z - input.Z);
        }
        public static Vector operator *(Vector current, Vector input)
        {
            return new Vector(current.X * input.X, current.Y * input.Y, current.Z * input.Z);
        }
        public static bool operator ==(Vector current, Vector input)
        {
            return (current - input).Length <= Maths.Common.DistanceEplision;
        }
        public static bool operator !=(Vector current, Vector input)
        {
            return (current - input).Length > Maths.Common.DistanceEplision;
        }
        #endregion

        public double ScalarProduct(Vector input)
        {
            return X * input.X + Y * input.Y + Z * input.Z;
        }
        public Vector VectorProduct(Vector input)
        {
            var x = Y * input.Z - Z * input.Y;
            var y = Z * input.X - X * input.Z;
            var z = X * input.Y - Y * input.X;
            return new Vector(x, y, z);
        }
        public bool IsParallel(Vector input)
        {
            var result = GetAngleInDegree(input);
            return Math.Abs(result) < Maths.Common.DistanceEplision 
                   || Math.Abs(result - 180.0) < Maths.Common.DistanceEplision;
        }
        public bool IsOrthogonal(Vector input)
        {
            var result = GetAngleInDegree(input);
            return Math.Abs(result - 90.0) < Maths.Common.DistanceEplision 
                   || Math.Abs(result - 270.0) < Maths.Common.DistanceEplision;
        }
        public double GetAngleIdRadian(Vector input)
        {
            var scalarProduct = ScalarProduct(input);
            var lengthProduct = this.Length * input.Length;
            var result = Math.Acos(scalarProduct / lengthProduct);
            return result;
        }
        public double GetAngleInDegree(Vector input)
        {
            var radian = GetAngleIdRadian(input);
            return radian * 180 / Math.PI;
        }
        #endregion Methods
    }
}