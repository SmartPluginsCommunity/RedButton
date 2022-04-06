using RedButton.Common.Core.Geometry.Extensions;
using RedButton.Common.Core.Geometry.Interfaces;

namespace RedButton.Common.Core.Geometry
{
    public class LineSegment : ILineSegment
    {
        public IPoint StartPoint { get; set; }
        public IPoint EndPoint { get; set; }

        public LineSegment(IPoint startPoint, IPoint endPoint)
        {
            StartPoint = startPoint.NewPoint();
            EndPoint = endPoint.NewPoint();
        }
        
        public LineSegment(IPoint startPoint, IVector vector)
        {
            StartPoint = startPoint.NewPoint();
            EndPoint = new Point(startPoint.NewPoint() + vector.NewPoint());
        }
    }
}