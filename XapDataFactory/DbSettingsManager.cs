using System;
using Xap.Infrastructure.Caches;
using Xap.Infrastructure.Configuration;
using Xap.Infrastructure.Environment;
using Xap.Infrastructure.Exceptions;
using Xap.Infrastructure.Interfaces.Data;
using Xap.Infrastructure.Logging;

namespace Xap.Data.Factory {
    internal class DbSettingsManager {
        #region "Constructors"
        private static readonly DbSettingsManager instance = new DbSettingsManager();

        static DbSettingsManager() {
        }

        private DbSettingsManager() {
        }

        public static DbSettingsManager Instance {
            get {
                return instance;
            }
        }
        #endregion

        private XapCache<string, IXapDbSettings> _dbSettings = new XapCache<string, IXapDbSettings>();
        internal IXapDbSettings GetDatabaseSettings(IXapDbContext dbContext) {
            string dbKey = string.Empty;
            try {
                dbKey = GetDbKey(dbContext.DbEnvironment, dbContext.DbConnectionName);
                IXapDbSettings dbSettings = _dbSettings.GetItem(dbKey);
                if (dbSettings == null) {
                    dbSettings = new DbSettings(dbContext);
                    dbSettings.DbKey = dbKey;
                    dbSettings.CommandTimeout = GetCommandTimeout(dbKey);
                    dbSettings.ConnectionString = GetConnectionString(dbKey);
                    dbSettings.DbHost = GetDbHost(dbKey);
                    dbSettings.DbName = GetDbName(dbKey);
                    dbSettings.DataProvider = GetDataProvider(dbKey);
                    dbSettings.SqlLocation = GetSqlLocation(dbKey);
                    dbSettings.MinPoolSize = GetMinPoolSize(dbKey);
                    dbSettings.MaxPoolSize = GetMaxPoolSize(dbKey);
                    dbSettings.PasswordProvider = GetPasswordProvider(dbKey);
                    dbSettings.Build();

                    _dbSettings.AddItem(dbSettings.DbKey, dbSettings);
                }
                return dbSettings;
            } catch (Exception ex) {
                XapLogger.Instance.Error($"Error retrieving database settings for {dbContext.DbEnvironment}:{dbContext.DbConnectionName}");
                XapLogger.Instance.Write(ex.Message);
                throw;
            }
        }

        internal void ClearDatabaseSettings() {
            _dbSettings.ClearCache();
        }

        private string GetDbKey(string environment, string subEnvironment) {
            string dKey = string.Empty;
            try {
                //check for a cached connection string for environemnt and sub environment
                if (_dbSettings.GetItem($"{XapEnvironment.Instance.EnvironmentName}.connectionStrings.{environment}.{subEnvironment}") != null) {
                    return $"{XapEnvironment.Instance.EnvironmentName}.connectionStrings.{environment}.{subEnvironment}";
                }

                //check config file for a connection string for environemnt and sub environment
                if (XapConfig.Instance.ContainsSection($"{XapEnvironment.Instance.EnvironmentName}.connectionStrings.{environment}.{subEnvironment}")) {
                    return $"{XapEnvironment.Instance.EnvironmentName}.connectionStrings.{environment}.{subEnvironment}";
                }

                //check for a cached connection string for the default environment
                if (_dbSettings.GetItem($"{XapEnvironment.Instance.EnvironmentName}.connectionStrings.{environment}.default") != null) {
                    return $"{XapEnvironment.Instance.EnvironmentName}.connectionStrings.{environment}.default";
                }

                //check config file for a connection string for the default environment
                if (XapConfig.Instance.ContainsSection($"{XapEnvironment.Instance.EnvironmentName}.connectionStrings.{environment}.default")) {
                    return $"{XapEnvironment.Instance.EnvironmentName}.connectionStrings.{environment}.default";
                }
                throw new XapException("No matching database configuration found.");
            } catch (Exception ex) {
                XapLogger.Instance.Error($"Error getting db key for {environment}:{subEnvironment}");
                XapLogger.Instance.Write(ex.Message);
                throw;
            }
        }

        private string GetConnectionString(string dbKey) {
            if (XapConfig.Instance.ContainsKey(dbKey, "connection")) {
                return XapConfig.Instance.GetValue<string>(dbKey, "connection");
            }
            return string.Empty;
        }

        private string GetDbHost(string dbKey) {
            if (XapConfig.Instance.ContainsKey(dbKey, "dbHost")) {
                return XapConfig.Instance.GetValue<string>(dbKey, "dbHost");
            }
            return string.Empty;
        }

        private string GetDbName(string dbKey) {
            if (XapConfig.Instance.ContainsKey(dbKey, "dbName")) {
                return XapConfig.Instance.GetValue<string>(dbKey, "dbName");
            }
            return string.Empty;
        }

        private string GetCommandTimeout(string dbKey) {
            if (XapConfig.Instance.ContainsKey(dbKey, "commandTimeout")) {
                return XapConfig.Instance.GetValue<string>(dbKey, "commandTimeout");
            }
            return "30";
        }

        private string GetMinPoolSize(string dbKey) {
            if (XapConfig.Instance.ContainsKey(dbKey, "minPoolSize")) {
                return XapConfig.Instance.GetValue<string>(dbKey, "minPoolSize");
            }
            return "100";
        }

        private string GetMaxPoolSize(string dbKey) {
            if (XapConfig.Instance.ContainsKey(dbKey, "maxPoolSize")) {
                return XapConfig.Instance.GetValue<string>(dbKey, "maxPoolSize");
            }
            return "1000";
        }

        private string GetPasswordProvider(string dbKey) {
            if (XapConfig.Instance.ContainsKey(dbKey, "passwordProvider")) {
                return XapConfig.Instance.GetValue<string>(dbKey, "passwordProvider");
            }
            return string.Empty;
        }

        private string GetDataProvider(string dbKey) {
            if (XapConfig.Instance.ContainsKey(dbKey, "dataProvider")) {
                return XapConfig.Instance.GetValue<string>(dbKey, "dataProvider");
            }
            return string.Empty;
        }

        private string GetSqlLocation(string dbKey) {
            if (XapConfig.Instance.ContainsKey(dbKey, "sqlLocation")) {
                return XapEnvironment.Instance.MapFolderPath(XapConfig.Instance.GetValue<string>(dbKey, "sqlLocation"));
            }
            return string.Empty;
        }
    }
}
