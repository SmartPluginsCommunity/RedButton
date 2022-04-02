using RedButton.Common.Core.Interfaces;

namespace RedButton.Common.Core.Extensions
{
    public static class VectorExtension
    {
        public static Vector GetVector(this IPoint input1, IPoint input2)
        {
            double x = input2.X - input2.X;
            double y = input2.Y - input2.Y;
            double z = input2.Z - input2.Z;
            return new Vector(x, y, z);
        }

        public static Vector NewVector(this IPoint input) => new Vector(input);
    }
}