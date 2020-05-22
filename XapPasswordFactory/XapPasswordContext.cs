using Xap.Password.Factory.Interfaces;

namespace Xap.Password.Factory {
    internal class XapPasswordContext:IXapPasswordContext{
        #region "Constructors"
        private XapPasswordContext() { }
        private XapPasswordContext(string configurationKey) {
            _configurationKey = configurationKey;
        }

        internal static IXapPasswordContext Create() {
            return new XapPasswordContext();
        }

        internal static IXapPasswordContext Create(string configurationKey) {
            return new XapPasswordContext(configurationKey);
        }
        #endregion

        #region "Properties"
        private string _password = string.Empty;
        string IXapPasswordContext.Password {
            get => _password;
            set => _password = value;
        }

        private string _vaultUserId = string.Empty;
        string IXapPasswordContext.VaultUserId {
            get => _vaultUserId;
            set => _vaultUserId = value;
        }

        private string _vaultKey = string.Empty;
        string IXapPasswordContext.VaultKey {
            get => _vaultKey;
            set => _vaultKey = value;
        }

        private string _vaultUrl = string.Empty;
        string IXapPasswordContext.VaultUrl {
            get => _vaultUrl;
            set => _vaultUrl = value;
        }

        private string _vaultAppId = string.Empty;
        string IXapPasswordContext.VaultAppId {
            get => _vaultAppId; 
            set => _vaultAppId = value; 
        }

        private string _vaultSafe = string.Empty;
        string IXapPasswordContext.VaultSafe {
            get =>  _vaultSafe; 
            set => _vaultSafe = value; 
        }

        private string _configurationKey = string.Empty;
        string IXapPasswordContext.ConfigurationKey {
            get => _configurationKey;
            set => _configurationKey = value;
        }

        private string _providerType = string.Empty;
        string IXapPasswordContext.ProviderType {
            get => _providerType;
            set => _providerType = value;
        }
        #endregion
    }
}
