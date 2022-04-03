using RedButton.Common.Core.Geometry.Extensions;
using RedButton.Common.Core.Geometry.Interfaces;

namespace RedButton.Common.Core.Geometry
{
    public class Sphere
    {
        #region Properties

        public Point Center { get; set; }
        public double Radius { get; set; }

        #endregion Properties

        #region Constructors

        public Sphere(IPoint center, double radius)
        {
            Center = center.NewPoint();
            Radius = radius;
        }
        public Sphere(Sphere input)
        {
            Center = input.Center.NewPoint();
            Radius = input.Radius;
        }
        

        public static Sphere operator +(Sphere sh, Point p) => new Sphere(sh.Center + p, sh.Radius);

        public static Sphere operator +(Sphere sh, double radius) => new Sphere(sh.Center, sh.Radius + radius);

        
        #endregion Constructors
        
        #region Methods

        public bool IsInsidePoint(Point input)
        {
            return (Center - input).Length <= Radius * Radius;
        }
        public bool IsIntersectionSphere(Sphere sh)
        {
            double dist = this.Distance(sh);
            double sum = Radius + sh.Radius;
            double diff = Math.Math.Abs(Radius - sh.Radius);
            
            bool inside = diff <= dist && dist <= sum;

            return inside;
        }

        public bool IsContact(Sphere sh)
        {
            return IsInside(sh) || IsIntersectionSphere(sh);
        }
        public bool IsInside(Sphere sh)
        {
            if (IsInsidePoint(sh.Center))
            {
                double dist = this.Distance(sh);
                double diff = Math.Math.Abs(Radius - sh.Radius);
                return dist < diff;
            }

            return false;
        }
        
        #endregion Methods
    }
}