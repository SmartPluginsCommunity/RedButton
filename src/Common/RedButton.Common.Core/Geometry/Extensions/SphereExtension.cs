using RedButton.Common.Core.Geometry.Interfaces;

namespace RedButton.Common.Core.Geometry.Extensions
{
    public static class SphereExtension
    {
        public static double Distance(this Sphere sphere, IPoint input) => sphere.Center.Distance(input);

        public static double Distance(this Sphere sphere, Sphere sh) => sphere.Center.Distance(sh.Center);
    }
}