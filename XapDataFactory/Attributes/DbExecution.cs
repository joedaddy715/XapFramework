namespace Xap.Data.Factory.Attributes {
    public class DbExecution : System.Attribute {
        private string _insertProcedure = string.Empty;
        public string InsertProcedure {
            get { return _insertProcedure; }
            set { _insertProcedure = value; }
        }

        private string _updateProcedure = string.Empty;
        public string UpdateProcedure {
            get { return _updateProcedure; }
            set { _updateProcedure = value; }
        }

        private string _selectProcedure = string.Empty;
        public string SelectProcedure {
            get { return _selectProcedure; }
            set { _selectProcedure = value; }
        }

        private string _selectListProcedure = string.Empty;
        public string SelectListProcedure {
            get { return _selectListProcedure; }
            set { _selectListProcedure = value; }
        }

        private string _deleteProcedure = string.Empty;
        public string DeleteProcedure {
            get { return _deleteProcedure; }
            set { _deleteProcedure = value; }
        }

        private string _dbEnvironment = string.Empty;
        public string DbEnvironment {
            get { return _dbEnvironment; }
            set { _dbEnvironment = value; }
        }

        private string _dbConnectionName = string.Empty;
        public string DbConnectionName {
            get { return _dbConnectionName; }
            set { _dbConnectionName = value; }
        }

        public DbExecution(string insert, string update, string select, string delete, string dbEnvironment, string dbConnectionName = "default") {
            _insertProcedure = insert;
            _updateProcedure = update;
            _selectProcedure = select;
            _deleteProcedure = delete;
            _dbEnvironment = dbEnvironment;
            _dbConnectionName = dbConnectionName;
        }
    }
}
