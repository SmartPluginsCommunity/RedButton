using System;
using System.Collections.Generic;

namespace RedButton.Common.Core.Geometry.Interfaces
{
    public interface IGeometryObject
    {
        IPoint CenterOfGravity {get;set;}
        List<IPoint> ListPoints {get;}
    }
}