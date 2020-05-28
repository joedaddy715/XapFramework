using Xap.Password.Factory.Interfaces;

namespace Xap.Password.Factory {
    /// <summary>
    ///  XapInfrastructure ProviderLoader.LoadProviders must be called before creating providers
    /// </summary>
    public class PasswordFactory {
        #region "Constructors"

        private static readonly PasswordFactory instance = new PasswordFactory();

        static PasswordFactory() { }

        private PasswordFactory() { }

        public static PasswordFactory Instance {
            get { return instance; }
        }
        #endregion

        #region "Password Context"
        public IXapPasswordContext PasswordContext() {
            return Factory.XapPasswordContext.Create();
        }

        public IXapPasswordContext PasswordContext(IXapPasswordContext passwordContext) {
            return PasswordContextBuilder.Create().PasswordContext(passwordContext);
        }

        /// <summary>
        /// will look up the context from a config file
        /// </summary>
        /// <param name="configurationKey"></param>
        /// <returns></returns>
        public IXapPasswordContext PasswordContext(string configurationKey) {
            return PasswordContextBuilder.Create().PasswordContext(configurationKey);
        }

        /// <summary>
        /// defaults to use tisam,   providers must be loaded before use
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="key"></param>
        /// <param name="uri">tisam uri</param>
        /// <returns></returns>
        public IXapPasswordContext PasswordContext(string userId, string key, string uri) {
            return PasswordContextBuilder.Create().PasswordContext(userId, key, uri);
        }

        /// <summary>
        /// defaults to use cyberArk,  providers must be loaded before use
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="uri"></param>
        /// <param name="safe"></param>
        /// <param name="appId"></param>
        /// <returns></returns>
        public IXapPasswordContext PasswordContext(string userId, string uri, string safe, string appId) {
            return PasswordContextBuilder.Create().PasswordContext(userId, uri,safe,appId);
        }
        #endregion
    }
}
