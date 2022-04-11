using RedButton.Common.Core.CollectionExtensions;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Tekla.Structures.Model;

namespace RedButton.Common.TeklaStructures.Model.Properties
{
    public class PropertiesExtractor
    {
        private ModelObject _modelObject;
        private Hashtable _values;

        private List<TeklaPropertyString> _stringAttributes;
        private List<TeklaPropertyDouble> _doubleAttributes;
        private List<TeklaPropertyInt> _integerAttributes;

        /// <summary>
        /// .ctor
        /// </summary>
        /// <param name="modelObject"></param>
        public PropertiesExtractor(ModelObject modelObject)
        {
            _modelObject = modelObject;
            _stringAttributes = new List<TeklaPropertyString>();
            _doubleAttributes = new List<TeklaPropertyDouble>();
            _integerAttributes = new List<TeklaPropertyInt>();
        }

        /// <summary>
        /// Add string attribute
        /// </summary>
        /// <param name="attribute"></param>
        public TeklaPropertyString AddStringAttribute(string attribute)
        {
            var property = new TeklaPropertyString(attribute, null);
            _stringAttributes.Add(property);
            return property;
        }

        /// <summary>
        ///  Add double attribute
        /// </summary>
        /// <param name="attribute"></param>
        public TeklaPropertyDouble AddDoubleAttribute(string attribute)
        {
            var property = new TeklaPropertyDouble(attribute, null);
            _doubleAttributes.Add(property);
            return property;
        }

        /// <summary>
        /// Add integer attribute
        /// </summary>
        /// <param name="attribute"></param>
        public TeklaPropertyInt AddIntegerAttribute(string attribute)
        {
            var property = new TeklaPropertyInt(attribute, null);
            _integerAttributes.Add(property);
            return property;
        }

        /// <summary>
        /// Refresh report properties
        /// </summary>
        public void ExtractProperties()
        {
            if (_modelObject == null)
            {
                _values = new Hashtable();
                return;
            }

            var valuesCount = _stringAttributes.Count + _doubleAttributes.Count + _integerAttributes.Count;

            _values = new Hashtable(valuesCount);

            _modelObject.GetAllReportProperties(_stringAttributes.Select(a => a.Name).ToArrayList(),
                                                _doubleAttributes.Select(a => a.Name).ToArrayList(),
                                                _integerAttributes.Select(a => a.Name).ToArrayList(),
                                                ref _values);

            foreach (var property in _stringAttributes)
                property.Value = GetExtractValue<string>(property.Name);

            foreach (var property in _doubleAttributes)
                property.Value = GetExtractValue<double>(property.Name);

            foreach (var property in _integerAttributes)
                property.Value = GetExtractValue<int>(property.Name);
        }

        /// <summary>
        /// Get value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        private T GetExtractValue<T>(string propertyName)
        {
            var val = _values[propertyName];
            return (T)val;
        }
    }
}
