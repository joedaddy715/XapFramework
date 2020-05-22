using Xap.Data.Factory.Interfaces;

namespace Xap.Data.Factory {
    internal class XapDbConnectionContext:IXapDbConnectionContext {
        #region "Constructors"
        private XapDbConnectionContext() : this(string.Empty,string.Empty, string.Empty, string.Empty, string.Empty,string.Empty,string.Empty) { }
        private XapDbConnectionContext(string environment) : this(environment, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty,string.Empty) { }
        private XapDbConnectionContext(string environment,string dbEnvironment) : this(environment,dbEnvironment, string.Empty, string.Empty, string.Empty,string.Empty,string.Empty) { }
        private XapDbConnectionContext(string environment,string dbEnvironment, string dbConnectionName) : this(environment,dbEnvironment,dbConnectionName,string.Empty, string.Empty,string.Empty,string.Empty) { }
        private XapDbConnectionContext(string environment, string dbEnvironment, string dbConnectionName,string tSql) : this(environment, dbEnvironment, dbConnectionName, tSql, string.Empty, string.Empty, string.Empty) { }
        private XapDbConnectionContext(string environment,string dbEnvironment, string dbConnectionName,string tSql,string connectionString) : this(environment,dbEnvironment, dbConnectionName, tSql,connectionString, string.Empty, string.Empty) { }
        private XapDbConnectionContext(string envrionment,string dbEnvironment, string dbConnectionName, string tSql, string connectionString,string userName, string password) {
            _userName = userName;
            _password = password;
            _environment = envrionment;
            _dbEnvironment = dbEnvironment;
            _dbConnectionName = dbConnectionName;
            _tSql = tSql;
            _connectionString = connectionString;
        }

        internal static IXapDbConnectionContext Create() {
            return new XapDbConnectionContext();
        }

        internal static IXapDbConnectionContext Create(string environment) {
            return new XapDbConnectionContext(environment);
        }

        internal static IXapDbConnectionContext Create(string environment,string dbEnvironment) {
            return new XapDbConnectionContext(environment,dbEnvironment);
        }

        internal static IXapDbConnectionContext Create(string environment, string dbEnvironment,string dbConnectionName) {
            return new XapDbConnectionContext(environment, dbEnvironment,dbConnectionName);
        }

        internal static IXapDbConnectionContext Create(string environment,string dbEnvironment,  string dbConnectionName,string tsql) {
            return new XapDbConnectionContext(environment,dbEnvironment, dbConnectionName,tsql);
        }

        internal static IXapDbConnectionContext Create(string environment,string dbEnvironment, string dbConnectionName, string tSql,string connectionString) {
            return new XapDbConnectionContext(environment,dbEnvironment, dbConnectionName, tSql,connectionString);
        }

        internal static IXapDbConnectionContext Create(string environment,string dbEnvironment, string dbConnectionName, string tSql,string connectionString, string userName, string password) {
            return new XapDbConnectionContext(environment,dbEnvironment, dbConnectionName, tSql,connectionString, userName, password);
        }

        #endregion

        #region "properties"
        private string _userName = string.Empty;
        string IXapDbConnectionContext.UserName {
            get => _userName;
            set => _userName = value;
        }

        private string _password = string.Empty;
        string IXapDbConnectionContext.Password {
            get => _password;
            set => _password = value;
        }

        private string _environment = string.Empty;
        string IXapDbConnectionContext.Environment {
            get => _environment;
            set => _environment = value;
        }

        private string _dbEnvironment = string.Empty;
        string IXapDbConnectionContext.DbEnvironment {
            get => _dbEnvironment;
            set => _dbEnvironment = value;
        }

        private string _dbConnectionName = string.Empty;
        string IXapDbConnectionContext.DbConnectionName {
            get => _dbConnectionName;
            set => _dbConnectionName = value;
        }

        private string _tSql = string.Empty;
        string IXapDbConnectionContext.TSql {
            get => _tSql;
            set => _tSql = value;
        }

        private string _connectionString = string.Empty;
        string IXapDbConnectionContext.ConnectionString {
            get => _connectionString;
            set => _connectionString = value;
        }

        private string _sqlLocation = string.Empty;
        string IXapDbConnectionContext.SqlLocation {
            get => _sqlLocation;
            set => _sqlLocation = value;
        }

        private string _dataProvider = string.Empty;
        string IXapDbConnectionContext.DataProvider {
            get => _dataProvider;
            set => _dataProvider = value;
        }

        private string _dbKey = string.Empty;
        string IXapDbConnectionContext.DbKey {
            get => _dbKey;
            set => _dbKey = value;
        }

        private string _dbHost = string.Empty;
        string IXapDbConnectionContext.DbHost {
            get => _dbHost;
            set => _dbHost = value;
        }

        private string _dbName = string.Empty;
        string IXapDbConnectionContext.DbName {
            get => _dbName;
            set => _dbName = value;
        }

        private string _commandTimeout = "30";
        string IXapDbConnectionContext.CommandTimeout {
            get => _commandTimeout;
            set => _commandTimeout = value;
        }

        private string _minPoolSize = "100";
        string IXapDbConnectionContext.MinPoolSize {
            get => _minPoolSize;
            set => _minPoolSize = value;
        }

        private string _maxPoolSize = "1000";
        string IXapDbConnectionContext.MaxPoolSize {
            get => _maxPoolSize;
            set => _maxPoolSize = value;
        }

        private string _port = string.Empty;
        string IXapDbConnectionContext.Port {
            get => _port;
            set => _port = value;
        }

        private string _passwordProvider = string.Empty;
        string IXapDbConnectionContext.PasswordProvider {
            get => _passwordProvider;
            set => _passwordProvider = value;
        }
        #endregion
    }
}
