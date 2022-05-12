using System.Collections.Generic;

namespace RedButton.Common.Core.Geometry.Interfaces
{
    public class Plate : IPlate
    {
        public Material Material { get; set; }
        public List<IPoint> ContourPoints { get; set; }
        public double Width { get; set; }

        public Plate(Material material, List<IPoint> contourPoints, double width)
        {
            Material = material;
            ContourPoints = contourPoints;
            Width = width;
        }
    }
}