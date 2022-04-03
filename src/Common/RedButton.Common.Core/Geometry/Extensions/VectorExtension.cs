using RedButton.Common.Core.Geometry.Interfaces;

namespace RedButton.Common.Core.Geometry.Extensions
{
    public static class VectorExtension
    {
        public static IVector GetVector(this IPoint input1, IPoint input2)
        {
            double x = input2.X - input2.X;
            double y = input2.Y - input2.Y;
            double z = input2.Z - input2.Z;
            return new Vector(x, y, z);
        }

        public static IVector NewVector(this IPoint input) => new Vector(input);
        public static IVector NewVector(this IVector input) => new Vector(input);
    }
}