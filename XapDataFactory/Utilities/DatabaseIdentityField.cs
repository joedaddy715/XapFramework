using System.Reflection;
using Xap.Data.Factory.Attributes;
using Xap.Infrastructure.Caches;
using Xap.Infrastructure.Services;

namespace Xap.Data.Factory.Utilities {
    public static class DatabaseIdentityField {
        public static PropertyInfo GetIdentityField<T>(T obj) {
            PropertyCache propertyCache = PropertyService.Instance.GetInterfaceProperties<T>(obj);
            foreach (PropertyInfo prop in propertyCache.GetProperties()) {
                object[] _attributes = prop.GetCustomAttributes(typeof(DbBinding), false);
                if (_attributes.Length > 0) {
                    if (_attributes[0] is DbBinding) {
                        if (((DbBinding)_attributes[0]).IsIdentityField) {
                            return prop;
                        }
                    }
                }
            }
            return null;
        }
    }
}
