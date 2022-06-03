using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace RedButton.Common.Core.Geometry.Interfaces
{
    public interface ISlab : IStructureElement
    {
        List<IPoint> ContourPoints { get; set; }
        double Width { get; set; }

        double Area { get; set; }
    }
}