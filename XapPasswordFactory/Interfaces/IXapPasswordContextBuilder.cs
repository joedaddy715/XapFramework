namespace Xap.Password.Factory.Interfaces {
    public interface IXapPasswordContextBuilder {
        IXapPasswordContext PasswordContext(string configurationKey);
        IXapPasswordContext PasswordContext(string userId, string key,string uri);
        IXapPasswordContext PasswordContext(string userId, string uri, string safe, string appId);
    }
}
