using System;
using System.Collections.Generic;

namespace RedButton.Common.Core.Geometry.Interfaces
{
    public interface IGeometryObject
    {
        IPoint CenterOfGravity {get;}
        List<IPoint> ListPoints {get;}

        IGeometryObject Clone();
    }
}