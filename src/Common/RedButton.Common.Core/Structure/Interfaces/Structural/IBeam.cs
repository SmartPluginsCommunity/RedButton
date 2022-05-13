namespace RedButton.Common.Core.Geometry.Interfaces
{
    public interface IBeam : IStructureElement
    {
        IPoint StartPoint { get; set; }
        IPoint EndPoint { get; set; }
        string Profile { get; set; }
        ProfileType ProfileType { get; set; }
    }
}