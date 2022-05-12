namespace RedButton.Common.Core.Geometry.Interfaces
{
    public interface IColumn : IStructureElement
    {
        IPoint StartPoint { get; set; }
        IPoint EndPoint { get; set; }
        string Profile { get; set; }
        ProfileType ProfileType { get; set; }
    }
}