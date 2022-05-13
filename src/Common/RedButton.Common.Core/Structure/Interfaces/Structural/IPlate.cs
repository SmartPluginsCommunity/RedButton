using System.Collections.Generic;

namespace RedButton.Common.Core.Geometry.Interfaces
{
    public interface IPlate: IStructureElement
    {
        List<IPoint> ContourPoints { get; set; }
        double Width { get; set; }
    }
}