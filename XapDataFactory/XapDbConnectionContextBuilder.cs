using System;
using Xap.Data.Factory.Interfaces;
using Xap.Infrastructure.Configuration;
using Xap.Infrastructure.Exceptions;
using Xap.Password.Factory;
using Xap.Password.Factory.Interfaces;

namespace Xap.Data.Factory {
    /// <summary>
    /// This implementation is built specifically for use with the Xap framework.
    /// You can create your own to handle your own connection strings
    /// </summary>
    internal class XapDbConnectionContextBuilder : IXapDbConnectionContextBuilder {
        #region "Constructors"
        private XapDbConnectionContextBuilder() { }
        public static IXapDbConnectionContextBuilder Create() {
            return new XapDbConnectionContextBuilder();
        }
        #endregion

        #region "Properties"
        private IXapDbConnectionContext dbConnectionContext = XapDbConnectionContext.Create();
        #endregion

        #region "Interface Methods"
        IXapDbConnectionContext IXapDbConnectionContextBuilder.DbConnectionContext() {
            return dbConnectionContext;
        }

        /// <summary>
        /// DbKey will be set to environment by default, used for cache key
        /// </summary>
        /// <param name="environment">application environment (dvlp,test,rlse,prod)</param>
        /// <returns>IXapDbConnectionContext</returns>
        IXapDbConnectionContext IXapDbConnectionContextBuilder.DbConnectionContext(string environment) {
            string dbKey = $"{environment}";

            dbConnectionContext = DbConnectionContextService.Instance.GetDbConnectionContext(dbKey);

            if (dbConnectionContext == null) {
                dbConnectionContext = DbFactory.Instance.DbConnectionContext();
                dbConnectionContext.DbKey = dbKey;
                dbConnectionContext.Environment = environment;
                ((IXapDbConnectionContextBuilder)this).CompileConnectionString();
                DbConnectionContextService.Instance.AddDbConnectionContext(dbConnectionContext.DbKey, dbConnectionContext);
            }
            
            return dbConnectionContext;
        }

        /// <summary>
        /// DbKey will be set to environment.dbEnvironment by default, used for cache key
        /// </summary>
        /// <param name="environment">application environment (dvlp,test,rlse,prod)</param>
        /// <param name="dbEnvironment">name under the connection strings section (connectionStrings.sql/sybase/facets/ccms/ca)</param>
        /// <returns>IXapDbConnectionContext</returns>
        IXapDbConnectionContext IXapDbConnectionContextBuilder.DbConnectionContext(string environment, string dbEnvironment) {
            string dbKey = $"{environment}.{dbEnvironment}";

            dbConnectionContext = DbConnectionContextService.Instance.GetDbConnectionContext(dbKey);

            if (dbConnectionContext == null) {
                dbConnectionContext = DbFactory.Instance.DbConnectionContext();
                dbConnectionContext.DbKey = dbKey;
                dbConnectionContext.Environment = environment;
                dbConnectionContext.DbEnvironment = dbEnvironment;
                ((IXapDbConnectionContextBuilder)this).CompileConnectionString();
                DbConnectionContextService.Instance.AddDbConnectionContext(dbConnectionContext.DbKey, dbConnectionContext);
            }

            return dbConnectionContext;
        }

        /// <summary>
        /// DbKey will be set to environment.dbEnvironment by default, used for cache key.  Default used for ACTS
        /// </summary>
        /// <param name="environment">application environment (dvlp,test,rlse,prod)</param>
        /// <param name="dbEnvironment">name under the connection strings section (connectionStrings.acts)</param>
        /// <param name="dbConnectionName">name to get the connection string (dvlp.connectionStrings.acts.default)</param>
        /// <returns>IXapDbConnectionContext</returns>
        IXapDbConnectionContext IXapDbConnectionContextBuilder.DbConnectionContext(string environment, string dbEnvironment, string dbConnectionName) {
            string dbKey = $"{environment}.connectionStrings.{dbEnvironment}.{dbConnectionName}";

            dbConnectionContext = DbConnectionContextService.Instance.GetDbConnectionContext(dbKey);

            if (dbConnectionContext == null) {
                dbConnectionContext = DbFactory.Instance.DbConnectionContext();
                dbConnectionContext.DbKey = dbKey;
                dbConnectionContext.Environment = environment;
                dbConnectionContext.DbEnvironment = dbEnvironment;
                dbConnectionContext.DbConnectionName = dbConnectionName;
                ((IXapDbConnectionContextBuilder)this).CompileConnectionString();
                DbConnectionContextService.Instance.AddDbConnectionContext(dbConnectionContext.DbKey, dbConnectionContext);
            }

            return dbConnectionContext;
        }

        /// <summary>
        /// DbKey will be set to environment.dbEnvironment.dbConnectionName by default, used for cache key.  Default used for ACTS
        /// </summary>
        /// <param name="environment">application environment (dvlp,test,rlse,prod)</param>
        /// <param name="dbEnvironment">name under the connection strings section (dvlp.connectionStrings.acts)</param>
        /// <param name="dbConnectionName">name to get the connection string (connectionStrings.acts.default)</param>
        /// <param name="tSql">sql code to execute (inline,file,procedure)</param>
        /// <returns>IXapDbConnectionContext</returns>
        IXapDbConnectionContext IXapDbConnectionContextBuilder.DbConnectionContext(string environment, string dbEnvironment, string dbConnectionName, string tSql) {
            string dbKey = $"{environment}.connectionStrings.{dbEnvironment}.{dbConnectionName}";

            dbConnectionContext = DbConnectionContextService.Instance.GetDbConnectionContext(dbKey);

            if (dbConnectionContext == null) {
                dbConnectionContext = DbFactory.Instance.DbConnectionContext();
                dbConnectionContext.DbKey = dbKey;
                dbConnectionContext.Environment = environment;
                dbConnectionContext.DbEnvironment = dbEnvironment;
                dbConnectionContext.DbConnectionName = dbConnectionName;
                dbConnectionContext.TSql = tSql;
                ((IXapDbConnectionContextBuilder)this).CompileConnectionString();
                DbConnectionContextService.Instance.AddDbConnectionContext(dbConnectionContext.DbKey, dbConnectionContext);
            }

            return dbConnectionContext;
        }

        /// <summary>
        /// DbKey will be set to environment.dbEnvironment.dbConnectionName by default, used for cache key.  Default used for ACTS
        /// </summary>
        /// <param name="environment">application environment (dvlp,test,rlse,prod)</param>
        /// <param name="dbEnvironment">name under the connection strings section (dvlp.connectionStrings.acts)</param>
        /// <param name="dbConnectionName">name to get the connection string (connectionStrings.acts.default)</param>
        /// <param name="tSql">sql code to execute (inline,file,procedure)</param>
        /// <param name="connectionString">connection string to use</param>
        /// <returns>IXapDbConnectionContext</returns>
        IXapDbConnectionContext IXapDbConnectionContextBuilder.DbConnectionContext(string environment, string dbEnvironment, string dbConnectionName, string tSql, string connectionString) {
            string dbKey = $"{environment}.connectionStrings.{dbEnvironment}.{dbConnectionName}";

            dbConnectionContext = DbConnectionContextService.Instance.GetDbConnectionContext(dbKey);

            if (dbConnectionContext == null) {
                dbConnectionContext = DbFactory.Instance.DbConnectionContext();
                dbConnectionContext.DbKey = dbKey;
                dbConnectionContext.Environment = environment;
                dbConnectionContext.DbEnvironment = dbEnvironment;
                dbConnectionContext.DbConnectionName = dbConnectionName;
                dbConnectionContext.TSql = tSql;
                dbConnectionContext.ConnectionString = connectionString;
                ((IXapDbConnectionContextBuilder)this).CompileConnectionString();
                DbConnectionContextService.Instance.AddDbConnectionContext(dbConnectionContext.DbKey, dbConnectionContext);
            }
            return dbConnectionContext;
        }

        /// <summary>
        /// DbKey will be set to environment.dbEnvironment.dbConnectionName by default, used for cache key.  Default used for ACTS
        /// </summary>
        /// <param name="environment">application environment (dvlp,test,rlse,prod)</param>
        /// <param name="dbEnvironment">name under the connection strings section (dvlp.connectionStrings.acts)</param>
        /// <param name="dbConnectionName">name to get the connection string (connectionStrings.acts.default)</param>
        /// <param name="tSql">sql code to execute (inline,file,procedure)</param>
        /// <param name="connectionString">connection string to use</param>
        /// <param name="userName">username for connection string</param>
        /// <param name="password">password for connection string</param>
        /// <returns>IXapDbConnectionContext</returns>
        IXapDbConnectionContext IXapDbConnectionContextBuilder.DbConnectionContext(string environment, string dbEnvironment, string dbConnectionName, string tSql, string connectionString, string userName, string password) {
            string dbKey = $"{environment}.{dbEnvironment}.connectionStrings.{dbConnectionName}";

            dbConnectionContext = DbConnectionContextService.Instance.GetDbConnectionContext(dbKey);

            if (dbConnectionContext == null) {
                dbConnectionContext = DbFactory.Instance.DbConnectionContext();
                dbConnectionContext.DbKey = dbKey;
                dbConnectionContext.Environment = environment;
                dbConnectionContext.DbEnvironment = dbEnvironment;
                dbConnectionContext.DbConnectionName = dbConnectionName;
                dbConnectionContext.TSql = tSql;
                dbConnectionContext.ConnectionString = connectionString;
                dbConnectionContext.UserName = userName;
                dbConnectionContext.Password = password;
                ((IXapDbConnectionContextBuilder)this).CompileConnectionString();
                DbConnectionContextService.Instance.AddDbConnectionContext(dbConnectionContext.DbKey, dbConnectionContext);
            }
            return dbConnectionContext;
        }

        void IXapDbConnectionContextBuilder.CompileConnectionString() {
            dbConnectionContext.CommandTimeout = GetCommandTimeout();
            dbConnectionContext.ConnectionString = GetConnectionString();
            dbConnectionContext.DbHost = GetDbHost();
            dbConnectionContext.DbName = GetDbName();
            dbConnectionContext.DataProvider = GetDataProvider();
            dbConnectionContext.SqlLocation = GetSqlLocation();
            dbConnectionContext.MinPoolSize = GetMinPoolSize();
            dbConnectionContext.MaxPoolSize = GetMaxPoolSize();
            dbConnectionContext.PasswordProvider = GetPasswordProvider();
            dbConnectionContext.Port = GetPort();
            dbConnectionContext.ConnectionString = BuildConnectionString();
        }
        #endregion

        #region "private methods"
        
        private string GetConnectionString() {
            if (XapConfig.Instance.ContainsKey(dbConnectionContext.DbKey, "connection")) {
                return XapConfig.Instance.GetValue<string>(dbConnectionContext.DbKey, "connection");
            }
            return string.Empty;
        }

        private string GetDbHost() {
            if (XapConfig.Instance.ContainsKey(dbConnectionContext.DbKey, "dbHost")) {
                return XapConfig.Instance.GetValue<string>(dbConnectionContext.DbKey, "dbHost");
            }
            return string.Empty;
        }

        private string GetDbName() {
            if (XapConfig.Instance.ContainsKey(dbConnectionContext.DbKey, "dbName")) {
                return XapConfig.Instance.GetValue<string>(dbConnectionContext.DbKey, "dbName");
            }
            return string.Empty;
        }

        private string GetCommandTimeout() {
            if (XapConfig.Instance.ContainsKey(dbConnectionContext.DbKey, "commandTimeout")) {
                return XapConfig.Instance.GetValue<string>(dbConnectionContext.DbKey, "commandTimeout");
            }
            return "30";
        }

        private string GetMinPoolSize() {
            if (XapConfig.Instance.ContainsKey(dbConnectionContext.DbKey, "minPoolSize")) {
                return XapConfig.Instance.GetValue<string>(dbConnectionContext.DbKey, "minPoolSize");
            }
            return "100";
        }

        private string GetMaxPoolSize() {
            if (XapConfig.Instance.ContainsKey(dbConnectionContext.DbKey, "maxPoolSize")) {
                return XapConfig.Instance.GetValue<string>(dbConnectionContext.DbKey, "maxPoolSize");
            }
            return "1000";
        }

        private string GetPasswordProvider() {
            if (XapConfig.Instance.ContainsKey(dbConnectionContext.DbKey, "passwordProvider")) {
                return XapConfig.Instance.GetValue<string>(dbConnectionContext.DbKey, "passwordProvider");
            }
            return string.Empty;
        }

        private string GetDataProvider() {
            if (XapConfig.Instance.ContainsKey(dbConnectionContext.DbKey, "dataProvider")) {
                return XapConfig.Instance.GetValue<string>(dbConnectionContext.DbKey, "dataProvider");
            }
            return string.Empty;
        }

        private string GetSqlLocation() {
            if (XapConfig.Instance.ContainsKey(dbConnectionContext.DbKey, "sqlLocation")) {
                return XapConfig.Instance.GetValue<string>(dbConnectionContext.DbKey, "sqlLocation");
            }
            return string.Empty;
        }

        private string GetPort() {
            if (XapConfig.Instance.ContainsKey(dbConnectionContext.DbKey, "port")) {
                return XapConfig.Instance.GetValue<string>(dbConnectionContext.DbKey, "port");
            }
            return string.Empty;
        }

        //TODO: create password factory for loading password providers, look at injecting
        private string BuildConnectionString() {
            try {
                if (dbConnectionContext.ConnectionString.Contains("[user]")) {
                    if (!string.IsNullOrEmpty(dbConnectionContext.UserName) && !string.IsNullOrEmpty(dbConnectionContext.Password)) {
                        dbConnectionContext.ConnectionString = dbConnectionContext.ConnectionString.Replace("[user]", dbConnectionContext.UserName);
                        dbConnectionContext.ConnectionString = dbConnectionContext.ConnectionString.Replace("[password]", dbConnectionContext.Password);
                    } else {
                        IXapPasswordContext pwdContext = PasswordContextBuilder.Create().PasswordContext($"{dbConnectionContext.DbKey}.passwordProvider");

                        dbConnectionContext.UserName = pwdContext.VaultUserId;
                        dbConnectionContext.Password = pwdContext.Password;

                        dbConnectionContext.ConnectionString = dbConnectionContext.ConnectionString.Replace("[user]", pwdContext.VaultUserId);
                        dbConnectionContext.ConnectionString = dbConnectionContext.ConnectionString.Replace("[password]", pwdContext.Password);
                    }
                }

                if (dbConnectionContext.ConnectionString.Contains("[dbHost]")) {
                    dbConnectionContext.ConnectionString = dbConnectionContext.ConnectionString.Replace("[dbHost]", dbConnectionContext.DbHost);
                }

                if (dbConnectionContext.ConnectionString.Contains("[dbName]")) {
                    dbConnectionContext.ConnectionString = dbConnectionContext.ConnectionString.Replace("[dbName]", dbConnectionContext.DbName);
                }

                if (dbConnectionContext.ConnectionString.Contains("[minPoolSize]")) {
                    dbConnectionContext.ConnectionString = dbConnectionContext.ConnectionString.Replace("[minPoolSize]", dbConnectionContext.MinPoolSize);
                }

                if (dbConnectionContext.ConnectionString.Contains("[maxPoolSize]")) {
                    dbConnectionContext.ConnectionString = dbConnectionContext.ConnectionString.Replace("[maxPoolSize]", dbConnectionContext.MaxPoolSize);
                }

                if (dbConnectionContext.ConnectionString.Contains("[port]")) {
                    dbConnectionContext.ConnectionString = dbConnectionContext.ConnectionString.Replace("[port]", dbConnectionContext.Port);
                }

                return dbConnectionContext.ConnectionString;
            } catch (Exception ex) {
                throw new XapException($"Error building connection string for {dbConnectionContext.DbKey}");
            }
        }
        #endregion
    }
}
