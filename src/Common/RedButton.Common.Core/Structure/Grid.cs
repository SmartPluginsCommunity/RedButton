using System.Collections.Generic;

namespace RedButton.Common.Core.Geometry.Interfaces
{
    public class Grid : IGrid
    {
        public IPoint AxisStartPoint { get; set; }
        public List<string> AxisX { get; set; }
        public List<string> AxisY { get; set; }
        public List<string> AxisZ { get; set; }
        public List<double> AxisesDistanceX { get; set; }
        public List<double> AxisesDistanceY { get; set; }
        public List<double> AxisesDistanceZ { get; set; }

        public Grid(IPoint axisStartPoint, List<string> axisX, List<string> axisY, List<string> axisZ, List<double> axisesDistanceX, List<double> axisesDistanceY, List<double> axisesDistanceZ)
        {
            AxisStartPoint = axisStartPoint;
            AxisX = axisX;
            AxisY = axisY;
            AxisZ = axisZ;
            AxisesDistanceX = axisesDistanceX;
            AxisesDistanceY = axisesDistanceY;
            AxisesDistanceZ = axisesDistanceZ;
        }
    }
}