using System;
using System.Collections.Generic;
using RedButton.Common.Core.Geometry.Enums;
using RedButton.Common.Core.Geometry.Extensions;
using RedButton.Common.Core.Geometry.Interfaces;

namespace RedButton.Common.Core.Geometry
{
    // Sphere 3d class
    public class Sphere : IGeometryObject
    {

        #region Properties
        /// <summary>
        /// Accuracy for 'ListPoints'
        /// </summary>
        public SphereAccuracy Accuracy = SphereAccuracy.degree90;
        
        private Point _center;
        public Point Center { 
            get => _center;
            private set => _center = value;
        }
        public double Radius { get; set; }
        public IPoint CenterOfGravity { get => _center; }
        public List<IPoint> ListPoints {
            get
            {
                var result = new List<IPoint>();
                result.Add(Center);
                if (Accuracy == SphereAccuracy.degree90)
                {
                    result.Add(_center.AddX(Radius));
                    result.Add(_center.AddX(-Radius));
                    result.Add(_center.AddY(Radius));
                    result.Add(_center.AddY(-Radius));
                    result.Add(_center.AddZ(Radius));
                    result.Add(_center.AddZ(-Radius));
                }
                return result;
            }
         }

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

        public IGeometryObject Clone()
        {
            return new Sphere(Center.NewVector(), (int)Radius) {Accuracy = this.Accuracy};
        }


        public bool IsInsidePoint(Point input)
        {
            var result = Center - input;
            var length = result.X * result.X + result.Y * result.Y + result.Z * result.Z;
            return length <= Radius * Radius;
        }
        public bool IsIntersectionSphere(Sphere sh)
        {
            double dist = this.Distance(sh);
            double sum = Radius + sh.Radius;
            double diff = Math.Abs(Radius - sh.Radius);
            
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
                double diff = Math.Abs(Radius - sh.Radius);
                return dist < diff;
            }

            return false;
        }
        
        #endregion Methods
    }
}