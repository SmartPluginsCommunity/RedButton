using RedButton.Common.Core.Geometry.Interfaces;

namespace RedButton.Common.Core.Geometry.Extensions
{
    public static class VectorExtension
    {
        public static IVector GetVector(this IPoint current, IPoint input)
        {
            double x = input.X - current.X;
            double y = input.Y - current.Y;
            double z = input.Z - current.Z;
            return new Vector(x, y, z);
        }

        public static IVector NewVector(this IPoint input) => new Vector(input);
        public static IVector NewVector(this IVector input) => new Vector(input);
    }
}