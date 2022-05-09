
using System; 
using Tekla.Structures;
using tsm = Tekla.Structures.Model;
using Tekla.Structures.Geometry3d;

namespace RedButton.Common.TeklaStructures.Extensions
{
    public static class VectorExtension
    {
        private const double TOLERANCE = 0.001;
        
        /// <summary>
        /// Return new Vector between two points
        /// </summary>
        /// <param name="v"></param>
        /// <param name="p1">Point p1</param>
        /// <param name="p2">Point p2</param>
        /// <returns></returns>
        public static Vector TwoPointVector(this Vector v, Point p1, Point p2)
        {
            return new Vector(p1.X - p2.X, p1.Y - p2.Y, p1.Z - p2.Z);
        }
        
        /// <summary>
        /// Converts the coordinates of the vector into positive
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static Vector Abs(this Vector v)
        {
            v.X = Math.Abs(v.X);
            v.Y = Math.Abs(v.Y);
            v.Z = Math.Abs(v.Z);
            return v;
        }
        /// <summary>
        /// Changes the direction of the vector in the opposite direction
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static Vector Opposite(this Vector v)
        {
            v.X = -1*v.X;
            v.Y = -1*v.Y;
            v.Z = -1*v.Z;
            return v;
        }
        
    }
}