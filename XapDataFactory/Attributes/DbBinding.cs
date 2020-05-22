namespace Xap.Data.Factory.Attributes {
    public class DbBinding : System.Attribute {
        private string _dbColumn = string.Empty;
        public string DbColumn {
            get { return _dbColumn; }
            set { _dbColumn = value; }
        }

        private bool _doesInsert = false;
        public bool DoesInsert {
            get { return _doesInsert; }
            set { _doesInsert = value; }
        }

        private bool _doesUpdate = false;
        public bool DoesUpdate {
            get { return _doesUpdate; }
            set { _doesUpdate = value; }
        }

        private bool _doesDelete = false;
        public bool DoesDelete {
            get { return _doesDelete; }
            set { _doesDelete = value; }
        }

        private bool _doesSelect = false;
        public bool DoesSelect {
            get { return _doesSelect; }
            set { _doesSelect = value; }
        }

        private bool _doesSelectList = false;
        public bool DoesSelectList {
            get { return _doesSelectList; }
            set { _doesSelectList = value; }
        }

        private string _dataType = string.Empty;
        public string DataType {
            get { return _dataType; }
            set { _dataType = value; }
        }

        private bool _isIdentityField = false;
        public bool IsIdentityField {
            get { return _isIdentityField; }
            set { _isIdentityField = value; }
        }

        public DbBinding(string dbColumn, bool insert, bool update, bool select, bool selectList, bool delete) : this(dbColumn, insert, update, select, selectList, delete, string.Empty, false) {
            _dbColumn = dbColumn;
            _doesInsert = insert;
            _doesUpdate = update;
            _doesDelete = delete;
            _doesSelect = select;
        }

        public DbBinding(string dbColumn, bool insert, bool update, bool select, bool selectList, bool delete, string dataType) : this(dbColumn, insert, update, select, selectList, delete, dataType, false) {
            _dbColumn = dbColumn;
            _doesInsert = insert;
            _doesUpdate = update;
            _doesDelete = delete;
            _doesSelect = select;
            _dataType = dataType;
        }

        public DbBinding(string dbColumn, bool insert, bool update, bool select, bool selectList, bool delete, string dataType, bool isIdentityField) {
            _dbColumn = dbColumn;
            _doesInsert = insert;
            _doesUpdate = update;
            _doesDelete = delete;
            _doesSelect = select;
            _doesSelectList = selectList;
            _isIdentityField = isIdentityField;
            _dataType = dataType;
        }
    }
}
