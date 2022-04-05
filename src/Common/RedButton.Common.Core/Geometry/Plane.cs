using RedButton.Common.Core.Geometry.Extensions;
using RedButton.Common.Core.Geometry.Interfaces;

namespace RedButton.Common.Core.Geometry
{
    public class Plane
    {
        #region Properties

        public IPoint Origin { get; set; }
        public IVector VectorX { get; set; }
        public IVector VectorY { get; set; }

        //TODO
        public IVector VectorZ;
        #endregion

        #region Constructor

        public Plane(IPoint origin, IVector vectorX, IVector vectorY)
        {
            Origin = origin.NewPoint();
            VectorX = vectorX.NewVector();
            VectorY = vectorY.NewVector();
        }

        #endregion Constructor

    }
}