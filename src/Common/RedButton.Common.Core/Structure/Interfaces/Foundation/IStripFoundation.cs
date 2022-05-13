using System.Collections.Generic;

namespace RedButton.Common.Core.Geometry.Interfaces.Foundation
{
    public interface IStripFoundation : IStructureElement
    {
        List<IPoint> StripFoundationPath { get; set; }
        string Profile { get; set; }
        ProfileType ProfileType { get; set; }  
    }
}