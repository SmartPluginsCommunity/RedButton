using System.Collections.Generic;

namespace RedButton.Common.Core.Geometry.Interfaces
{
    public class Slab : ISlab
    {
        public List<IPoint> ContourPoints { get; set; }
        public Material Material { get; set; }
        public double Width { get; set; }
        public double Area { get; set; }

        public Slab(List<IPoint> contourPoints, double width)
        {
            ContourPoints = contourPoints;
            Width = width;
        }
    }
}