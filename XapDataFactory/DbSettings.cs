using System;
using Xap.Infrastructure.Exceptions;
using Xap.Infrastructure.Interfaces.Data;
using Xap.Infrastructure.Interfaces.Security;
using Xap.Infrastructure.Logging;
using Xap.Infrastructure.Providers;

namespace Xap.Data.Factory {
    internal class DbSettings : IXapDbSettings {
        private IXapDbContext _dbContext;

        internal DbSettings(IXapDbContext dbContext) {
            _dbContext = dbContext;
        }

        private string _dbKey = string.Empty;
        string IXapDbSettings.DbKey {
            get => _dbKey;
            set => _dbKey = value;
        }

        private string _connectionString = string.Empty;
        string IXapDbSettings.ConnectionString {
            get => _connectionString;
            set => _connectionString = value;
        }

        private string _dbHost = string.Empty;
        string IXapDbSettings.DbHost {
            get => _dbHost;
            set => _dbHost = value;
        }

        private string _dbName = string.Empty;
        string IXapDbSettings.DbName {
            get => _dbName;
            set => _dbName = value;
        }

        private string _sqlLocation = string.Empty;
        string IXapDbSettings.SqlLocation {
            get => _sqlLocation;
            set => _sqlLocation = value;
        }

        private string _commandTimeout = "30";
        string IXapDbSettings.CommandTimeout {
            get => _commandTimeout;
            set => _commandTimeout = value;
        }

        private string _dataProvider = string.Empty;
        string IXapDbSettings.DataProvider {
            get => _dataProvider;
            set => _dataProvider = value;
        }

        private string _minPoolSize = "100";
        string IXapDbSettings.MinPoolSize {
            get => _minPoolSize;
            set => _minPoolSize = value;
        }

        private string _maxPoolSize = "1000";
        string IXapDbSettings.MaxPoolSize {
            get => _maxPoolSize;
            set => _maxPoolSize = value;
        }

        private string _port = string.Empty;
        string IXapDbSettings.Port {
            get => _port;
            set => _port = value;
        }

        private string _passwordProvider = string.Empty;
        string IXapDbSettings.PasswordProvider {
            get => _passwordProvider;
            set => _passwordProvider = value;
        }

        IXapDbSettings IXapDbSettings.Build() {
            try {

                //no connection string found, error
                if (string.IsNullOrWhiteSpace(_connectionString)) {
                    throw new XapException("Invalid database environment.  Please check your application configuration file.");
                }


                if (_connectionString.Contains("[user]")) {
                    if (_dbContext != null) {
                        if (!string.IsNullOrEmpty(_dbContext.UserName) && !string.IsNullOrEmpty(_dbContext.Password)) {
                            _connectionString = _connectionString.Replace("[user]", _dbContext.UserName);
                            _connectionString = _connectionString.Replace("[password]", _dbContext.Password);
                        } else {
                            IXapPasswordProvider pwd = ProviderLoader.Instance.LoadPasswordProvider(_passwordProvider);
                            pwd.RetrievePassword(_dbKey);

                            if (string.IsNullOrEmpty(pwd.VaultUserId) || string.IsNullOrEmpty(pwd.Password)) {
                                XapLogger.Instance.Error("Error retrieving Db Credentials");
                                throw new XapException("Error retrieving Db Credentials");
                            }

                            _connectionString = _connectionString.Replace("[user]", pwd.VaultUserId);
                            _connectionString = _connectionString.Replace("[password]", pwd.Password);
                        }
                    }
                }

                if (_connectionString.Contains("[dbHost]")) {
                    _connectionString = _connectionString.Replace("[dbHost]", _dbHost);
                }

                if (_connectionString.Contains("[dbName]")) {
                    _connectionString = _connectionString.Replace("[dbName]", _dbName);
                }

                if (_connectionString.Contains("[minPoolSize]")) {
                    _connectionString = _connectionString.Replace("[minPoolSize]", _minPoolSize.ToString());
                }

                if (_connectionString.Contains("[maxPoolSize]")) {
                    _connectionString = _connectionString.Replace("[maxPoolSize]", _maxPoolSize.ToString());
                }

                if (_connectionString.Contains("[port]")) {
                    _connectionString = _connectionString.Replace("[port]", _port);
                }
                return this;
            } catch (Exception ex) {
                XapLogger.Instance.Error($"Error building DBSettings");
                XapLogger.Instance.Write(ex.Message);
                throw;
            }
        }
    }
}
