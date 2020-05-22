namespace Xap.Data.Factory.Interfaces {
    public interface IXapDbConnectionContextBuilder{
        IXapDbConnectionContext DbConnectionContext();

        IXapDbConnectionContext DbConnectionContext(string environment);

        IXapDbConnectionContext DbConnectionContext(string environment, string dbEnvironment);

        IXapDbConnectionContext DbConnectionContext(string environment, string dbEnvironment, string dbConnectionName);

        IXapDbConnectionContext DbConnectionContext(string environment, string dbEnvironment, string dbConnectionName, string tSql);
        IXapDbConnectionContext DbConnectionContext(string environment, string dbEnvironment, string dbConnectionName, string tSql,string connectionString);

        IXapDbConnectionContext DbConnectionContext(string environment, string dbEnvironment, string dbConnectionName, string tSql, string connectionString, string userName, string password);
        void CompileConnectionString();
    }
}
