namespace Xap.Password.Factory.Interfaces {
    public interface IXapPasswordContext {
        string VaultAppId { get; set; }
        string VaultSafe { get; set; }
        string VaultUserId { get; set; }
        string Password { get; set; }
        string VaultKey { get; set; }
        string VaultUrl { get; set; }
        string ProviderType { get; set; }
        string ConfigurationKey { get; set; }
    }
}
