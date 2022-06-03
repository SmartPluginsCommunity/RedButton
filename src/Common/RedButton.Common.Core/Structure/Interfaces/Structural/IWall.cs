namespace RedButton.Common.Core.Geometry.Interfaces
{
    public interface IWall : IStructureElement
    {
        IPoint StartPoint { get; set; }
        IPoint EndPoint { get; set; }
        double StartHeight { get; set; }
        double EndHeight { get; set; }
        double Width { get; set; }
    }
}