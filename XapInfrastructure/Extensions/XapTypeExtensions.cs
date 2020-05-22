using System;
using System.Linq;
using System.Reflection;
using Xap.Infrastructure.Logging;

namespace Xap.Infrastructure.Extensions {
    public static class XapTypeExtensions {
        public static object GetDefaultValue(this Type t) {
            if (t.IsValueType)
                return Activator.CreateInstance(t);

            return null;
        }
        public static T ConvertValue<T>(this object value) {
            try {
                return (T)Convert.ChangeType(value, typeof(T));
            } catch {
                XapLogger.Instance.Error($"Error converting {value}");
                throw;
            }
        }

        public static string ShortName(this PropertyInfo property) {
            return property.Name.Split('.').Last();
        }
    }
}
