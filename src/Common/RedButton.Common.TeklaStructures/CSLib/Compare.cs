namespace RedButton.Common.TeklaStructures.CSLib
{
    public class Compare
    {
        public static bool GT(double a, double b) => a - b > Constants.CS_EPSILON;

        public static bool LT(double a, double b) => GT(b, a);

        public static bool GE(double a, double b) => !LT(a, b);

        public static bool LE(double a, double b) => !GT(a, b);

        public static bool IN(double a, double b, double c) => GE(a, b) && LT(a, c);

        public static bool IX(double a, double b, double c) => GT(a, b) && LE(a, c);

        public static bool EQ(double a, double b) => GE(a, b) && LE(a, b);

        public static bool NE(double a, double b) => !EQ(a, b);

        public static bool ZR(double a) => EQ(a, 0.0);

        public static bool NZ(double a) => !ZR(a);

        public static void Swap<T>(ref T X, ref T Y)
        {
            T obj = X;
            X = Y;
            Y = obj;
        }
    }
}