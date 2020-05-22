using Xap.Infrastructure.Interfaces.Configuration;
using Xap.Infrastructure.Providers;

namespace Xap.Infrastructure.Configuration {
    public class XapConfig {
        #region "Constructors"

        private static readonly IXapConfigurationProvider instance = ProviderLoader.Instance.LoadConfigurationProvider();

        static XapConfig() { }

        private XapConfig() { }

        public static IXapConfigurationProvider Instance {
            get { return instance; }
        }
        #endregion
    }
}
