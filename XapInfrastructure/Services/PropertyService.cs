using System;
using System.Linq;
using System.Reflection;
using Xap.Infrastructure.Caches;
using Xap.Infrastructure.Exceptions;

namespace Xap.Infrastructure.Services {
    public class PropertyService {
        #region "Constructors"

        private static readonly PropertyService instance = new PropertyService();

        static PropertyService() { }

        private PropertyService() { }

        public static PropertyService Instance {
            get { return instance; }
        }
        #endregion

        /// <summary>
        /// Assumes interface for object follows naming convention of IObjectName ex: Person:IPerson
        /// </summary>
        /// <typeparam name="TObject"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        //TODO:  expand to handle getting properties for class and single or all interfaces
        public PropertyCache GetProperties<TObject>(TObject obj) {
            try {
                string componentInterface = $"I{obj.GetType().FullName.Split('.').Last()}";

                PropertyCache pCache = PropertyCache.Create();

                PropertyInfo[] props = typeof(TObject).GetProperties(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);

                foreach (PropertyInfo prop in props) {
                    if (prop.Name.Contains(componentInterface)) {
                        pCache.AddProperty(prop);
                    }
                }

                return pCache;
            } catch (Exception ex) {
                throw new XapException($"Error getting properties for {typeof(TObject).FullName}", ex);
            }
        }
    }
}
