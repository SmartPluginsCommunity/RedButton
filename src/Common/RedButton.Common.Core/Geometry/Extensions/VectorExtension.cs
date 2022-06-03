using System;
using RedButton.Common.Core.Geometry.Interfaces;

namespace RedButton.Common.Core.Geometry.Extensions
{
    public static class VectorExtension
    {
        public static Vector GetVector(this IPoint current, IPoint input)
        {
            double x = input.X - current.X;
            double y = input.Y - current.Y;
            double z = input.Z - current.Z;
            return new Vector(x, y, z);
        }
        public static Vector NewVector(this IPoint input) => new Vector(input);
        public static Vector NewVector(this IVector input) => new Vector(input);
        public static Vector AddX(this IVector current, double input)
        {
            return new Vector(current.X + input, current.Y, current.Z);
        }
        public static Vector AddY(this IVector current, double input)
        {
            return new Vector(current.X, current.Y + input, current.Z);
        }
        public static Vector AddZ(this IVector current, double input)
        {
            return new Vector(current.X, current.Y, current.Z + input);
        }
        public static Vector Offset(this IVector current, IVector direction, double length)
        {
            var line = direction.NewVector() - current.NewVector();
            line.Normalize();
            line.X *= length;
            line.Y *= length;
            line.Z *= length;
            return current.NewVector() + line;
        }
        public static Vector Normalize(this IVector current)
        {
            var mod = current.Modulus();
            double modInv = 1 / mod;
            return new Vector(current.X * modInv, current.Y * modInv, current.Z * modInv);
        }
        public static double Modulus(this IVector current)
        {
            var vector = current.NewVector();
            return Math.Sqrt(vector.ScalarProduct(vector));
        }
        public static double Distance(this IVector current, IVector input)
        {
            var resultVector = input.NewVector() - current.NewVector();
            return resultVector.Length;
        }
        public static Vector Multiply(this IVector current, double value)
        {
            return new Vector(current.X * value, current.Y * value, current.Z * value);
        }
    }
}