namespace Xap.Infrastructure.Interfaces.Security {
    public interface IXapSecurityProvider {
        IXapUser XapUser { get; }
        IXapUser InitializeUser(string userName, string password);
    }
}
