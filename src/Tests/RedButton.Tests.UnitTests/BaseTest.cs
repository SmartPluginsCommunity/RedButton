using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Tekla.Structures.Geometry3d;
using Tekla.Structures.Model;

namespace RedButton.Tests.UnitTests
{
    public abstract class BaseTest
    {
        private ConcurrentBag<ModelObject> _insertedObjects;
        
        protected Model Model;
        /// <summary>
        /// Initial logic
        /// </summary>
        [TestInitialize]
        public void Init()
        {
            Model = new Model();

            if (!Model.GetConnectionStatus())
                throw new Exception("Connect to Tekla model status is false!");

            if(_insertedObjects == null)
                _insertedObjects = new ConcurrentBag<ModelObject>();
        }

        [TestCleanup]
        public void Cleanup()
        {
            foreach(var mo in _insertedObjects)
                mo?.Delete();

            Model.CommitChanges();

            _insertedObjects = null;
        }

        protected Beam GetBeam()
        {
            var beam = new Beam();
            beam.Profile.ProfileString = "D100";
            beam.StartPoint = new Point();
            beam.EndPoint = new Point(1000, 0,0 );

            if (beam.Insert())
            {
                _insertedObjects.Add(beam);
                return beam;
            }

            return null;
        }

        protected Assembly CreateAssembly(Part mainPart, IEnumerable<Part> secondaries)
        {
            var assembly = mainPart.GetAssembly();

            foreach(var part in secondaries)
                assembly.Add(part);

            return assembly;
        }
    }
}
