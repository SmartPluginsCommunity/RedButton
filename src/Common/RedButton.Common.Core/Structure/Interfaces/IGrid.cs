using System.Collections.Generic;

namespace RedButton.Common.Core.Geometry.Interfaces
{
    public interface IGrid
    {
        IPoint AxisStartPoint { get; set; }
        List<string> AxisX { get; set; }
        List<string> AxisY { get; set; }
        List<string> AxisZ { get; set; }

        List<double> AxisesDistanceX { get; set; }
        List<double> AxisesDistanceY { get; set; }
        List<double> AxisesDistanceZ { get; set; }
    }
}