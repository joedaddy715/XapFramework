using System;
using System.Collections.Generic;
using System.Reflection;
using Xap.Infrastructure.Extensions;

namespace Xap.Infrastructure.Caches {
    [Serializable]
    public class PropertyCache {
        #region "Constructors"
        private PropertyCache() { }

        public static PropertyCache Create() {
            return new PropertyCache();
        }
        #endregion

        #region "Properties"
        private XapCache<string, PropertyInfo> _properties = new XapCache<string, PropertyInfo>();
        #endregion

        #region "Public Methods"
        public IEnumerable<PropertyInfo> GetProperties() {
            foreach (KeyValuePair<string, PropertyInfo> kvp in _properties.GetItems()) {
                yield return kvp.Value; ;
            }
        }

        public PropertyCache AddProperty(PropertyInfo property) {
            _properties.AddItem(property.ShortName(), property);
            return this;
        }

        public PropertyCache AddProperty(string propertyName, PropertyInfo property) {
            _properties.AddItem(propertyName, property);
            return this;
        }

        public PropertyInfo GetProperty(string propertyName) {
            return _properties.GetItem(propertyName);
        }

        public void Clear() {
            _properties.ClearCache();
        }

        public int Count {
            get => _properties.Count;
        }
        #endregion
    }
}
