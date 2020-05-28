using System;
using System.Linq;
using System.Reflection;
using Xap.Infrastructure.Exceptions;

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
                throw new XapException($"Error converting {value}");
            }
        }

        public static string ShortName(this PropertyInfo property) {
            return property.Name.Split('.').Last();
        }
    }
}
