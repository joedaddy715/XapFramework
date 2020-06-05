using System;
using System.Data;
using Xap.Data.Factory.Interfaces;
using Xap.Data.Factory.Poco;
using Xap.Infrastructure.AppDomain;
using Xap.Infrastructure.Environment;
using Xap.Infrastructure.Exceptions;
using Xap.Password.Factory.Interfaces;

namespace Xap.Data.Factory {
    /// <summary>
    /// XapInfrastructure ProviderLoader.LoadProviders must be called before creating connections
    /// For Use with the XapFramework
    /// </summary>
    /// TODO: look at making interface based to allow other implementations
    public class DbFactory {
        #region "Constructors"

        private static readonly DbFactory instance = new DbFactory();

        static DbFactory() { }

        private DbFactory() { }

        public static DbFactory Instance {
            get { return instance; }
        }
        #endregion

        #region "Provider Loader"
        internal IXapDataConnectionProvider LoadDataConnectionProvider(string providerType) {
            try {
                return AssemblyManager.Instance.CreateInstance<IXapDataConnectionProvider>(providerType);
            } catch(Exception ex) {
                throw new XapException($"Error loading data provider {providerType}", ex);
            }
        }
        #endregion

        #region #DbProvider"
        public IXapDataProvider Db(IXapDbConnectionContext dbConnectionContext) {
            return XapDb.Create(dbConnectionContext);
        }

        public IXapDataProvider Db(IXapDbConnectionContext dbConnectionContext,IXapPasswordContext passwordContext) {
            dbConnectionContext.UserName = passwordContext.VaultUserId;
            dbConnectionContext.Password = passwordContext.Password;
            return XapDb.Create(dbConnectionContext);
        }

        public IXapDataProvider Db(string dbEnvironment) {
            return XapDb.Create(XapDbConnectionContextBuilder.Create().DbConnectionContext(dbEnvironment));
        }

        public IXapDataProvider Db(string dbEnvironment, string dbConnectionName) {
            return XapDb.Create(XapDbConnectionContextBuilder.Create().DbConnectionContext(XapEnvironment.Instance.EnvironmentName, dbEnvironment, dbConnectionName));
        }

        public IXapDataProvider Db(string dbEnvironment, string dbConnectionName, string tSql) {
            return XapDb.Create(XapDbConnectionContextBuilder.Create().DbConnectionContext(XapEnvironment.Instance.EnvironmentName, dbEnvironment, dbConnectionName, tSql));
        }

        public IXapDataProvider Db(string dbEnvironment, string dbConnectionName, string tSql,string connectionString) {
            return XapDb.Create(XapDbConnectionContextBuilder.Create().DbConnectionContext(XapEnvironment.Instance.EnvironmentName, dbEnvironment, dbConnectionName, tSql,connectionString));
        }

        public IXapDataProvider Db(string dbEnvironment, string dbConnectionName, string tSql,string connectionString, string userName, string password) {
            return XapDb.Create(XapDbConnectionContextBuilder.Create().DbConnectionContext(XapEnvironment.Instance.EnvironmentName, dbEnvironment, dbConnectionName, tSql,connectionString, userName, password));
        }
        #endregion

        #region "Poco"
        public IXapPoco Poco(IXapDbConnectionContext dbConnectionContext) {
            return XapPoco.Create(dbConnectionContext);
        }

        public IXapPoco Poco(string dbEnvironment) {
            return XapPoco.Create(XapDbConnectionContextBuilder.Create().DbConnectionContext(dbEnvironment));
        }

        public IXapPoco Poco(string dbEnvironment, string dbConnectionName) {
            return XapPoco.Create(XapDbConnectionContextBuilder.Create().DbConnectionContext(XapEnvironment.Instance.EnvironmentName, dbEnvironment, dbConnectionName));
        }

        public IXapPoco Poco(string dbEnvironment, string dbConnectionName,string tSql) {
            return XapPoco.Create(XapDbConnectionContextBuilder.Create().DbConnectionContext(XapEnvironment.Instance.EnvironmentName, dbEnvironment, dbConnectionName, tSql));
        }

        public IXapPoco Poco(string dbEnvironment, string dbConnectionName, string tSql,string connectionString) {
            return XapPoco.Create(XapDbConnectionContextBuilder.Create().DbConnectionContext(XapEnvironment.Instance.EnvironmentName, dbEnvironment, dbConnectionName, tSql, connectionString));
        }

        public IXapPoco Poco(string dbEnvironment, string dbConnectionName, string tSql,string connectionString, string userName, string password) {
            return XapPoco.Create(XapDbConnectionContextBuilder.Create().DbConnectionContext(XapEnvironment.Instance.EnvironmentName, dbEnvironment, dbConnectionName, tSql, connectionString, userName, password));
        }
        #endregion

        #region "DbConnectionContext"
        public  IXapDbConnectionContext DbConnectionContext() {
            return XapDbConnectionContext.Create();
        }

        public IXapDbConnectionContext DbConnectionContext(string environment) {
            return XapDbConnectionContext.Create(environment);
        }

        public IXapDbConnectionContext DbConnectionContext(string environment, string dbEnvironment) {
            return XapDbConnectionContext.Create(environment, dbEnvironment);
        }

        public IXapDbConnectionContext DbConnectionContext(string environment,string dbEnvironment, string dbConnectionName) {
            return XapDbConnectionContext.Create(environment,dbEnvironment, dbConnectionName);
        }

        public IXapDbConnectionContext DbConnectionContext(string environment,string dbEnvironment, string dbConnectionName, string tSql) {
            return XapDbConnectionContext.Create(environment, dbEnvironment, dbConnectionName, tSql);
        }

        public IXapDbConnectionContext DbConnectionContext(string environment, string dbEnvironment, string dbConnectionName, string tSql,string connectionString) {
            return XapDbConnectionContext.Create(environment, dbEnvironment, dbConnectionName, tSql, connectionString);
        }

        public IXapDbConnectionContext DbConnectionContext(string environment,string dbEnvironment, string dbConnectionName, string tSql,string connectionString, string userName, string password) {
            return XapDbConnectionContext.Create(environment,dbEnvironment, dbConnectionName, tSql,connectionString, userName, password);
        }
        #endregion

        #region "DbParameter"
        public IXapDbParameter DbParameter() {
            return XapDbParameter.Create();
        }

        public  IXapDbParameter DbParameter(string parameterName, object parameterValue, ParameterDirection parameterDirection = ParameterDirection.Input) {
            return XapDbParameter.Create(parameterName, parameterValue, parameterDirection);
        }
        #endregion
    }
}
