using System;

namespace Xap.Infrastructure.Shared {
    public static class SharedMethods {
        //TODO: Test property cache changes
        public static T GetCustomAttribute<T>(object obj) where T : System.Attribute {
            Type _type = obj.GetType();
            T[] attribs = _type.GetCustomAttributes(typeof(T), false) as T[];
            if (attribs.Length > 0) {
                return attribs[0];
            }
            return default(T);
        }
    }
}
