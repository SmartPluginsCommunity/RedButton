using System;
using System.Collections.Generic;
using System.Linq;
using RedButton.Common.Core.Geometry.Extensions;
using RedButton.Common.Core.Geometry.Interfaces;

namespace RedButton.Common.Core.Geometry
{
    public class Shape : IGeometryObject
    {
        #region Properties
        private List<IGeometryObject> _objects;
        public List<IGeometryObject> Objects { 
            get => _objects;
            set => _objects = value;
        }

        public IPoint CenterOfGravity
        {
            get
            {
                return Objects
                    .Select(x => x.CenterOfGravity)
                    .ToList()
                    .GetCenterPoint();
            }
        }

        public List<IPoint> ListPoints
        {
            get
            {
                return Objects.SelectMany(x => x.ListPoints).ToList();
            }
        }
        public IGeometryObject Clone()
        {
            return new Shape(this);
        }

        #endregion Properties

        #region Constructors

        public Shape(){}
        public Shape(Shape input)
        {
            this.Objects = input.Objects;
        }

        #endregion Constructors

        #region Methods

        public void SetObjects(List<IGeometryObject> input) => Objects = input;


        #endregion Methods
    }
}