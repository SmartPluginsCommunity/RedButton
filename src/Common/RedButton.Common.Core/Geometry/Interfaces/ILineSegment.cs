namespace RedButton.Common.Core.Geometry.Interfaces
{
    public interface ILineSegment
    {
        IPoint StartPoint { get; set; }
        IPoint EndPoint { get; set; }
    }
}