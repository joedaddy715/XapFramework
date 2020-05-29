using Xap.Infrastructure.AppDomain;
using Xap.Infrastructure.Configuration.Interfaces;
using Xap.Infrastructure.Environment;

namespace Xap.Infrastructure.Configuration {
    internal class ConfigFactory {
        #region "Constructors"
        private static readonly ConfigFactory instance = new ConfigFactory();

        static ConfigFactory() { }

        private ConfigFactory() { }

        internal static ConfigFactory Instance {
            get { return instance; }
        }
        #endregion

        #region "Loaders"
        /// <summary>
        /// This method reads from the app.config and loads the default configuration needed to work with the configuration provider.
        /// This is called by XapConfig and used as the default
        /// </summary>
        /// <returns></returns>
        internal IXapConfigurationProvider InitializeConfigurationProvider() {
            try {
                AssemblyManager.Instance.ClearCache();
                AssemblyManager.Instance.LoadAssemblies(XapEnvironment.Instance.GetAppConfigValue("configurationProviderPath"), "IXapConfigurationProvider");
                return AssemblyManager.Instance.CreateInstance<IXapConfigurationProvider>(XapEnvironment.Instance.GetAppConfigValue("configurationProvider"));
            } catch {
                throw;
            }
        }
        #endregion
    }
}
