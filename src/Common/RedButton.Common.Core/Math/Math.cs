using System;

/*
 * Math.Pow - from https://habr.com/ru/post/584662/
 */

namespace RedButton.Common.Core.Math
{
    public static class Math
    {
        public static double Sqrt(double value) => Pow(value, 0.5);

        public static double Pow(double b, double e)
        {
            long i = BitConverter.DoubleToInt64Bits(b);
            i = (long)(4606853616395542500L + e * (i - 4606853616395542500L));
            return BitConverter.Int64BitsToDouble(i);
        }

        public static double Abs(double value)
        {
            if (value >= 0) return value;
            return -1 * value;
        }
    }
}