using Xap.Infrastructure.Configuration.Interfaces;

namespace Xap.Infrastructure.Configuration {
    public class XapConfig {
        #region "Constructors"

        private static readonly IXapConfigurationProvider instance = ConfigFactory.Instance.InitializeConfigurationProvider();

        static XapConfig() { }

        private XapConfig() { }

        public static IXapConfigurationProvider Instance {
            get { return instance; }
        }
        #endregion
    }
}
