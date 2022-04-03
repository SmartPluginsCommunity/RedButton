using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Concurrent;
using Tekla.Structures.Model;

namespace RedButton.Tests.UnitTests
{
    public abstract class BaseTest
    {
        private ConcurrentBag<ModelObject> _insertedObjects;

        protected TestObjectCreator TestObjectCreator { get; set; }

        protected Model Model;
        /// <summary>
        /// Initial logic
        /// </summary>
        [TestInitialize]
        public void Init()
        {
            Model = new Model();
            TestObjectCreator = new TestObjectCreator();

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

        protected void AddTemporaryObject(ModelObject modelObject)
        {
            _insertedObjects.Add(modelObject);
        }
    }
}
