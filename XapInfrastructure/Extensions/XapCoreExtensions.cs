using System;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using Xap.Infrastructure.Caches;
using Xap.Infrastructure.Core;
using Xap.Infrastructure.Logging;
using Xap.Infrastructure.Services;

namespace Xap.Infrastructure.Extensions {
    public static class XapCoreExtensions {
        public static T Clone<T>(this T source) {
            if (!typeof(T).IsSerializable) {
                XapLogger.Instance.Error($"{typeof(T).FullName} must be marked as serializable");
                throw new ArgumentException($"{typeof(T).FullName} must be marked as serializable", "source");
            }

            // Don't serialize a null object, simply return the default for that object
            if (Object.ReferenceEquals(source, null)) {
                return default(T);
            }

            IFormatter formatter = new BinaryFormatter();
            Stream stream = new MemoryStream();
            using (stream) {
                formatter.Serialize(stream, source);
                stream.Seek(0, SeekOrigin.Begin);
                return (T)formatter.Deserialize(stream);
            }
        }

        public static string GetNameSpace(this object source) {
            Type t = source.GetType();
            return t.FullName;
        }

        public static T GetCustomAttribute<T>(this object obj) where T : System.Attribute {
            Type _type = obj.GetType();
            T[] attribs = _type.GetCustomAttributes(typeof(T), false) as T[];
            if (attribs.Length > 0) {
                return attribs[0];
            }
            return default(T);
        }

        //TODO:  Change to use NewtonSoft and append other objects
        public static string ToJson<TObject>(this TObject obj)

            where TObject : XapObjectCore {

            PropertyCache props = PropertyService.Instance.GetProperties(obj);

            // Start the JSON object.
            StringBuilder json = new StringBuilder();
            json.AppendLine("{\"data\":{");

            // Add the properties.
            foreach (PropertyInfo prop in props.GetProperties()) {
                json.AppendLine($"\"{prop.ShortName()}\":\"{prop.GetValue(obj, null).ToString()}\",");
            }

            json = json.RemoveLast(",");

            // Finish the JSON object.
            json.AppendLine("}}");
            return json.ToString();

        }
    }
}
