using System.Collections.Generic;
using System.Linq;
using Xap.Data.Factory.Interfaces;
using Xap.Infrastructure.Caches;

namespace Xap.Data.Factory.Poco {
    internal class PocoMap : IXapPocoMap {
        #region "Constructors"
        private PocoMap() { }

        internal static PocoMap Create() {
            return new PocoMap();
        }
        #endregion

        #region "Properties/Builder Methods"
        private readonly XapCache<string, IXapPocoField> _fieldList = new XapCache<string, IXapPocoField>();

        private string _objectName = string.Empty;
        string IXapPocoMap.ObjectName {
            get => _objectName;
            set => _objectName = value;
        }

        private string _selectProcedure = string.Empty;
        string IXapPocoMap.SelectProcedure {
            get => _selectProcedure;
            set => _selectProcedure = value;
        }

        private string _selectListProcedure = string.Empty;
        string IXapPocoMap.SelectListProcedure {
            get => _selectListProcedure;
            set => _selectListProcedure = value;
        }
        private string _insertProcedure = string.Empty;
        string IXapPocoMap.InsertProcedure {
            get => _insertProcedure;
            set => _insertProcedure = value;
        }

        private string _updateProcedure = string.Empty;
        string IXapPocoMap.UpdateProcedure {
            get => _updateProcedure;
            set => _updateProcedure = value;
        }

        private string _deleteProcedure = string.Empty;
        string IXapPocoMap.DeleteProcedure {
            get => _deleteProcedure;
            set => _deleteProcedure = value;
        }
        #endregion

        #region "Field Methods"
        void IXapPocoMap.AddField(IXapPocoField pocoField) {
            _fieldList.AddItem(pocoField.FieldName.Split('.').Last(), pocoField);
        }

        IXapPocoField IXapPocoMap.GetField(string fieldName) {
            return _fieldList.GetItem(fieldName);
        }

        string IXapPocoMap.GetIdentityField() {
            foreach (KeyValuePair<string, IXapPocoField> kvp in _fieldList.GetItems()) {
                if (kvp.Value.IsIdentity) {
                    return kvp.Value.FieldName;
                }
            }
            return string.Empty;
        }
        #endregion
    }
}
