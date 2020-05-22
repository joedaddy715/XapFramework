namespace Xap.Data.Factory.Interfaces {
    public interface IXapDbConnectionContext {
        string Environment { get; set; }
        string DbEnvironment { get; set; }
        string DbConnectionName { get; set; }
        string UserName { get; set; }
        string Password { get; set; }
        string TSql { get; set; }
        string ConnectionString { get; set; }
        string SqlLocation { get; set; }
        string DataProvider { get; set; }
        string DbKey { get; set; }
        string CommandTimeout { get; set; }
        string DbHost { get; set; }
        string DbName { get; set; }
        string MinPoolSize { get; set; }
        string MaxPoolSize { get; set; }
        string PasswordProvider { get; set; }
        string Port { get; set; }
    }
}
