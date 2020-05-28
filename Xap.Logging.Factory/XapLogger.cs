using Xap.Logging.Factory.Interfaces;

namespace Xap.Logging.Factory {
    public class XapLogger {
        #region "Constructors"

        private static readonly IXapLoggingProvider instance = LogFactory.Instance.LoadDefaultLoggingProvider();

        static XapLogger() { }

        private XapLogger() { }

        public static IXapLoggingProvider Instance {
            get { return instance; }
        }
        #endregion
    }
}
