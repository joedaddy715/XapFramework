using System;
using Xap.Infrastructure.AppDomain;
using Xap.Infrastructure.Configuration;
using Xap.Infrastructure.Exceptions;
using Xap.Password.Factory.Interfaces;
using Xap.Password.Factory.Providers;

namespace Xap.Password.Factory {
    public class PasswordContextBuilder:IXapPasswordContextBuilder {
        #region "Constructors"
        private PasswordContextBuilder() { }
        public static IXapPasswordContextBuilder Create() {
            return new PasswordContextBuilder();
        }
        #endregion

        #region "Properties"
        private IXapPasswordContext passwordContext = XapPasswordContext.Create();
        #endregion

        #region "interface methods"
        /// <summary>
        /// manually fill in the password context information needed to retrieve the password
        /// </summary>
        /// <param name="passwordContext"></param>
        /// <returns></returns>
        IXapPasswordContext IXapPasswordContextBuilder.PasswordContext(IXapPasswordContext xapPasswordContext) {
            passwordContext = PasswordContextService.Instance.GetPasswordContext(xapPasswordContext.VaultUserId);

            if (passwordContext == null) {
                GetPassword();

                PasswordContextService.Instance.AddPasswordContext(xapPasswordContext.VaultUserId, passwordContext);
            }
            return passwordContext;
        }

        /// <summary>
        /// will look up the context from a config file
        /// </summary>
        /// <param name="configurationKey"></param>
        /// <returns></returns>
        IXapPasswordContext IXapPasswordContextBuilder.PasswordContext(string configurationKey) {
            passwordContext = PasswordContextService.Instance.GetPasswordContext(configurationKey);

            if (passwordContext == null) {
                passwordContext = PasswordFactory.Instance.PasswordContext();
                passwordContext.ConfigurationKey = configurationKey;
                passwordContext.ProviderType = GetProviderType();
                passwordContext.VaultAppId = GetVaultAppId();
                passwordContext.VaultKey = GetVaultKey();
                passwordContext.VaultSafe = GetVaultSafe();
                passwordContext.VaultUrl = GetVaultUri();
                passwordContext.VaultUserId = GetVaultUserId();

                GetPassword();

                PasswordContextService.Instance.AddPasswordContext(configurationKey, passwordContext);
            }
            return passwordContext;
        }

        /// <summary>
        /// defaults to use tisam,   providers must be loaded before use
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="key"></param>
        /// <param name="uri">tisam uri</param>
        /// <returns></returns>
        IXapPasswordContext IXapPasswordContextBuilder.PasswordContext(string userId, string key,string uri) {
            passwordContext = PasswordContextService.Instance.GetPasswordContext(userId);

            if (passwordContext == null) {
                passwordContext = PasswordFactory.Instance.PasswordContext();
                passwordContext.ConfigurationKey = userId;
                passwordContext.VaultUserId = userId;
                passwordContext.VaultUrl = uri;
                passwordContext.ProviderType = PasswordProviderType.Tisam;
                passwordContext.VaultKey = key;

                GetPassword();

                PasswordContextService.Instance.AddPasswordContext(userId, passwordContext);
            }
            return passwordContext;
        }

        /// <summary>
        /// defaults to use cyberArk,  providers must be loaded before use
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="uri"></param>
        /// <param name="safe"></param>
        /// <param name="appId"></param>
        /// <returns></returns>
        IXapPasswordContext IXapPasswordContextBuilder.PasswordContext(string userId, string uri, string safe, string appId) {
            passwordContext = PasswordContextService.Instance.GetPasswordContext(userId);

            if (passwordContext == null) {
                passwordContext = PasswordFactory.Instance.PasswordContext();
                passwordContext.ConfigurationKey = userId;
                passwordContext.ProviderType = PasswordProviderType.CyberArk;
                passwordContext.VaultAppId = appId;
                passwordContext.VaultSafe = safe;
                passwordContext.VaultUrl = uri;
                passwordContext.VaultUserId = userId;

                GetPassword();

                PasswordContextService.Instance.AddPasswordContext(userId, passwordContext);
            }
            return passwordContext;
        }
        #endregion

        #region "private methods"
        private string GetVaultUri() {
            if (XapConfig.Instance.ContainsKey(passwordContext.ConfigurationKey, "vaultUri")) {
                return XapConfig.Instance.GetValue<string>(passwordContext.ConfigurationKey, "vaultUri");
            }
            return string.Empty;
        }

        private string GetVaultAppId() {
            if (XapConfig.Instance.ContainsKey(passwordContext.ConfigurationKey, "vaultAppId")) {
                return XapConfig.Instance.GetValue<string>(passwordContext.ConfigurationKey, "vaultAppId");
            }
            return string.Empty;
        }

        private string GetVaultSafe() {
            if (XapConfig.Instance.ContainsKey(passwordContext.ConfigurationKey, "vaultSafe")) {
                return XapConfig.Instance.GetValue<string>(passwordContext.ConfigurationKey, "vaultSafe");
            }
            return string.Empty;
        }
        private string GetVaultUserId() {
            if (XapConfig.Instance.ContainsKey(passwordContext.ConfigurationKey, "vaultUserId")) {
                return XapConfig.Instance.GetValue<string>(passwordContext.ConfigurationKey, "vaultUserId");
            }
            return string.Empty;
        }

        private string GetVaultKey() {
            if (XapConfig.Instance.ContainsKey(passwordContext.ConfigurationKey, "vaultKey")) {
                return XapConfig.Instance.GetValue<string>(passwordContext.ConfigurationKey, "vaultKey");
            }
            return string.Empty;
        }

        private string GetProviderType() {
            if (XapConfig.Instance.ContainsKey(passwordContext.ConfigurationKey, "providerType")) {
                return XapConfig.Instance.GetValue<string>(passwordContext.ConfigurationKey, "providerType");
            }
            return string.Empty;
        }
        #endregion

        #region "Provider Loader"
        public void GetPassword() {
            IXapPasswordProvider pwdProvider = LoadPasswordProvider(passwordContext.ProviderType);
            passwordContext = pwdProvider.RetrievePassword(passwordContext);
        }

        internal IXapPasswordProvider LoadPasswordProvider(string providerType) {
            try {
                return AssemblyManager.Instance.CreateInstance<IXapPasswordProvider>(providerType);
            } catch (Exception ex) {
                throw new XapException($"Error loading password provider {providerType}", ex);
            }
        }
        #endregion
    }
}
