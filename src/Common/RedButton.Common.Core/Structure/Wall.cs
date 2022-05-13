namespace RedButton.Common.Core.Geometry.Interfaces
{
    public class Wall : IWall
    {
        public Material Material { get; set; }
        public IPoint StartPoint { get; set; }
        public IPoint EndPoint { get; set; }
        public double StartHeight { get; set; }
        public double EndHeight { get; set; }
        public double Width { get; set; }

        public Wall(Material material, IPoint startPoint, IPoint endPoint, double startHeight, double endHeight, double width)
        {
            Material = material;
            StartPoint = startPoint;
            EndPoint = endPoint;
            StartHeight = startHeight;
            EndHeight = endHeight;
            Width = width;
        }
    }
}