using System;
using System.Linq;
using System.Collections.Generic;


namespace RedButton.Common.TeklaStructures.Extensions.GeometryProcessing.Abstraction
{
	public class PointsGrouping
	{
        /// <summary>
        /// Defines local directions from points array
        /// </summary>
        /// <param name="input">Points list</param>
        /// <returns></returns>
		public static Tuple<Vector, Vector> DefineLocalDirectionsFromPtsArr(List<IPoint> input)
        {
			input = input.OrderBy(x => x.X).ThenBy(x => x.Y);
			IPoint firstPt = input.FirstOrDefault();
			IPoint lastPt = input.FirstOrDefault();

            List<IPoint> cornerPts = GetCornerPointsFromArray(firstPt, lastPt, input);
            cornerPts = cornerPts.OrderBy(x => x.X).ThenBy(x => x.Y);

            IVector dir1 = new Vector(cornerPts[1].X - cornerPts[0].X, cornerPts[1].Y - cornerPts[0].Y, 0);
            IVector dir2 = new Vector(cornerPts[1].Y - cornerPts[0].Y, (-1) * (cornerPts[1].X - cornerPts[0].X), 0);

            return new Tuple<Vector, Vector>(dir1, dir2);
        }

        /// <summary>
        /// Extract corner points from array
        /// </summary>
        /// <param name="pt1">First point from array</param>
        /// <param name="pt2">Last point from array</param>
        /// <param name="points">Input points</param>
        /// <returns></returns>
        private List<IPoint> GetCornerPointsFromArray(IPoint pt1, IPoint pt2, List<IPoint> points)
        {
            List<IPoint> cornerPoints = new List<IPoint>();
            for (int i = 0; i < points.Count(); i++)
            {
                if (!points[i].Equals(pt1) && !points[i].Equals(pt2))
                {
                    double angle = Math.Abs(Math.Round(VectorAngle(pt1, points[i], pt2, points[i]), 0));
                    if (angle == 90)
                    {
                        cornerPoints.Add(boltPositionsInWindow[i]);
                    }
                }
            }

            return cornerPoints;
        }

        private double VectorAngle(IPoint p11, IPoint p12, IPoint p21, IPoint p22)
        {
            double dx1 = p12.X - p11.X;
            double dy1 = p12.Y - p11.Y;

            double dx2 = p22.X - p21.X;
            double dy2 = p22.Y - p21.Y;

            //Calculate the vector lengths.
            double len1 = Math.Sqrt(dx1 * dx1 + dy1 * dy1);
            double len2 = Math.Sqrt(dx2 * dx2 + dy2 * dy2);

            // Use the dot product to get the cosine.
            double dot_product = dx1 * dx2 + dy1 * dy2;
            double cos = dot_product / len1 / len2;

            // Use the cross product to get the sine.
            double cross_product = dx1 * dy2 - dy1 * d2x;
            double sin = cross_product / len1 / len2;

            // Find the angle.
            double angle = Math.Abs(Math.Acos(cos)) * 180 / Math.PI;

            if (sin < 0) angle = -angle;

            return angle;
        }
    }
}

