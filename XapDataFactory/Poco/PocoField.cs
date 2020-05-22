using Xap.Data.Factory.Interfaces;

namespace Xap.Data.Factory.Poco {
    internal class PocoField : IXapPocoField {
        #region "Constructors"
        private PocoField() { }

        internal static IXapPocoField Create() {
            return new PocoField();
        }
        #endregion

        #region "Properties/Builder Methods"
        private string _fieldName = string.Empty;
        string IXapPocoField.FieldName {
            get => _fieldName;
            set => _fieldName = value;
        }

        private bool _doesInsert = false;
        bool IXapPocoField.DoesInsert {
            get => _doesInsert;
            set => _doesInsert = value;
        }

        private bool _doesSelect = false;
        bool IXapPocoField.DoesSelect {
            get => _doesSelect;
            set => _doesSelect = value;
        }

        private bool _doesSelectList = false;
        bool IXapPocoField.DoesSelectList {
            get => _doesSelectList;
            set => _doesSelectList = value;
        }

        private bool _doesUpdate = false;
        bool IXapPocoField.DoesUpdate {
            get => _doesUpdate;
            set => _doesUpdate = value;
        }

        private bool _doesDelete = false;
        bool IXapPocoField.DoesDelete {
            get => _doesDelete;
            set => _doesDelete = value;
        }

        private bool _isIdentity = false;
        bool IXapPocoField.IsIdentity {
            get => _isIdentity;
            set => _isIdentity = value;
        }

        private string _dbColumn = string.Empty;
        string IXapPocoField.DbColumn {
            get => _dbColumn;
            set => _dbColumn = value;
        }

        private string _dataType = string.Empty;
        string IXapPocoField.DataType {
            get => _dataType;
            set => _dataType = value;
        }
        #endregion
    }
}
