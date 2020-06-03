using System.Reflection;
using Xap.Data.Factory.Attributes;
using Xap.Data.Factory.Interfaces;
using Xap.Infrastructure.Caches;
using Xap.Infrastructure.Services;
using Xap.Infrastructure.Shared;

namespace Xap.Data.Factory.Poco {
    internal class PocoMapService {
        #region "Constructors"

        private static readonly PocoMapService instance = new PocoMapService();

        static PocoMapService() { }

        private PocoMapService() { }

        internal static PocoMapService Instance {
            get { return instance; }
        }
        #endregion

        #region "Properties"
        private XapCache<string, IXapPocoMap> pocoMaps = new XapCache<string, IXapPocoMap>();

        #endregion

        #region "public methods"
        internal IXapPocoMap GetPocoMap<T>(T obj) {

            IXapPocoMap pocoMap = pocoMaps.GetItem(typeof(T).FullName);
            if (pocoMap != null) {
                return pocoMap;
            }

            DbExecution dbExecution = SharedMethods.GetCustomAttribute<DbExecution>(obj);
            if (dbExecution == null) {
                return null;
            }
            pocoMap = PocoMap.Create();
            pocoMap.ObjectName = obj.GetType().FullName;
            pocoMap.InsertProcedure = dbExecution.InsertProcedure;
            pocoMap.SelectProcedure = dbExecution.SelectProcedure;
            pocoMap.SelectListProcedure = dbExecution.SelectListProcedure;
            pocoMap.UpdateProcedure = dbExecution.UpdateProcedure;
            pocoMap.DeleteProcedure = dbExecution.DeleteProcedure;

            foreach (PropertyInfo prop in PropertyService.Instance.GetInterfaceProperties<T>(obj).GetProperties()) {
                object[] attributes = prop.GetCustomAttributes(typeof(DbBinding), true);
                if (attributes.Length == 1) {
                    IXapPocoField pocoField = PocoField.Create();
                    pocoField.DbColumn = ((DbBinding)attributes[0]).DbColumn;
                    pocoField.FieldName = prop.Name;
                    pocoField.DoesInsert = ((DbBinding)attributes[0]).DoesInsert;
                    pocoField.DoesSelect = ((DbBinding)attributes[0]).DoesSelect;
                    pocoField.DoesSelectList = ((DbBinding)attributes[0]).DoesSelectList;
                    pocoField.DoesUpdate = ((DbBinding)attributes[0]).DoesUpdate;
                    pocoField.DoesDelete = ((DbBinding)attributes[0]).DoesDelete;
                    pocoField.IsIdentity = ((DbBinding)attributes[0]).IsIdentityField;
                    pocoField.DataType = ((DbBinding)attributes[0]).DataType;

                    pocoMap.AddField(pocoField);
                }
            }

            pocoMaps.AddItem(pocoMap.ObjectName, pocoMap);
            return pocoMap;
        }
        #endregion
    }
}
