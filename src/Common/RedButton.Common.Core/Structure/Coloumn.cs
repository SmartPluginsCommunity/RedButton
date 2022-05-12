namespace RedButton.Common.Core.Geometry.Interfaces
{
    public class Coloumn : IColumn
    {
        public Material Material { get; set; }
        public IPoint StartPoint { get; set; }
        public IPoint EndPoint { get; set; }
        public string Profile { get; set; }
        public ProfileType ProfileType { get; set; }

        public Coloumn(Material material, IPoint startPoint, IPoint endPoint, string profile, ProfileType profileType)
        {
            Material = material;
            StartPoint = startPoint;
            EndPoint = endPoint;
            Profile = profile;
            ProfileType = profileType;
        }
    }
}