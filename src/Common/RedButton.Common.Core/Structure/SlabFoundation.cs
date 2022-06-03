using System.Collections.Generic;
using RedButton.Common.Core.Geometry.Interfaces.Foundation;

namespace RedButton.Common.Core.Geometry.Interfaces
{
    public class SlabFoundation : ISlabFoundation
    {
        public Material Material { get; set; }
        public List<IPoint> ContourPoints { get; set; }
        public double Width { get; set; }
        public double Area { get; set; }

        public SlabFoundation(Material material, List<IPoint> contourPoints, double width, double area)
        {
            Material = material;
            ContourPoints = contourPoints;
            Width = width;
            Area = area;
        }
    }
}