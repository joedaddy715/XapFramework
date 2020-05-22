using Xap.Infrastructure.Interfaces.Logging;
using Xap.Infrastructure.Providers;

namespace Xap.Infrastructure.Logging {
    public class XapLogger {
        #region "Constructors"

        private static readonly IXapLoggingProvider instance = ProviderLoader.Instance.LoadLoggingProvider();

        static XapLogger() { }

        private XapLogger() { }

        public static IXapLoggingProvider Instance {
            get { return instance; }
        }
        #endregion
    }
}
