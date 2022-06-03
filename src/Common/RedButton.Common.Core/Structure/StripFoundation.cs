using System.Collections.Generic;
using RedButton.Common.Core.Geometry.Interfaces.Foundation;

namespace RedButton.Common.Core.Geometry.Interfaces
{
    public class StripFoundation : IStripFoundation
    {
        public Material Material { get; set; }
        public List<IPoint> StripFoundationPath { get; set; }
        public string Profile { get; set; }
        public ProfileType ProfileType { get; set; }

        public StripFoundation(Material material, List<IPoint> stripFoundationPath, string profile, ProfileType profileType)
        {
            Material = material;
            StripFoundationPath = stripFoundationPath;
            Profile = profile;
            ProfileType = profileType;
        }
    }
}