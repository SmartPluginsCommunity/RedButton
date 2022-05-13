using RedButton.Common.Core.Geometry.Interfaces.Foundation;

namespace RedButton.Common.Core.Geometry.Interfaces
{
    public class Pile :IPile
    {
        public Material Material { get; set; }
        public IPoint DownPoint { get; set; }
        public IPoint UpPoint { get; set; }
        public ProfileType ProfileType { get; set; }

        public Pile(Material material, IPoint downPoint, IPoint upPoint, ProfileType profileType)
        {
            Material = material;
            DownPoint = downPoint;
            UpPoint = upPoint;
            ProfileType = profileType;
        }
    }
}