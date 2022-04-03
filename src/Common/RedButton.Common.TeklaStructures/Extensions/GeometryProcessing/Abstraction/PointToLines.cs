using System;

namespace RedButton.Common.TeklaStructures.Extensions.GeometryProcessing.Abstraction
{
    public static class PointToLines
    {
        /// <summary>
        /// Creates point along line with given offset from start or end of the line
        /// </summary>
        /// <param name="startPt">Start point instance</param>
        /// <param name="endPt">End point instance</param>
        /// <param name="pointOffset">Offset value along line</param>
        /// <param name="swapDir"> false - offset from start point, true - offset from end point</param>
        /// <returns></returns>
        public static IPoint OffsetPointAlongLine(this IPoint startPt, IPoint endPt, double pointOffset,
            bool swapDir = false)
        {
            double drob = Math.Pow(pointOffset, 2) / (Math.Pow(startPt.PointX - endPt.PointX, 2) +
                                                      Math.Pow(startPt.PointY - endPt.PointY, 2) +
                                                      Math.Pow(startPt.PointZ - endPt.PointZ, 2));
            double diskrem = 4 - 4 * (1 - drob);
            double p1 = (2 + Math.Pow(diskrem, 0.5)) / 2;
            double p2 = (2 - Math.Pow(diskrem, 0.5)) / 2;
            double p = 0.0;

            if (0 < p1 && p1 < 1)
            {
                p = p1;
            }

            else if (0 < p2 && p2 < 1)
            {
                p = p2;
            }

            double dx1 = 0.0;
            double dx2 = 0.0;


            if (swapDir == false)
            {
                dx1 = p;
                dx2 = 1 - p;
            }

            else
            {
                dx1 = 1 - p;
                dx2 = p;
            }

            double xNewCoord = dx1 * startPt.PointX + dx2 * endPt.PointX;
            double yNewCoord = dx1 * startPt.PointY + dx2 * endPt.PointY;
            double zNewCoord = dx1 * startPt.PointZ + dx2 * endPt.PointZ;

            IPoint point = new Point()
            {
                PointX = xNewCoord,
                PointY = yNewCoord,
                PointZ = zNewCoord
            };

            return point;
        }
    }
}

