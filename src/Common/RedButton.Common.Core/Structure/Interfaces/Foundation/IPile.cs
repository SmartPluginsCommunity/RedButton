using System.Collections.Specialized;

namespace RedButton.Common.Core.Geometry.Interfaces.Foundation
{
    public interface IPile : IStructureElement
    {
        IPoint DownPoint { get; set; }
        IPoint UpPoint { get; set; }
        ProfileType ProfileType { get; set; }
    }
}