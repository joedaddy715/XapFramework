using Xap.Infrastructure.Interfaces.Security;
using Xap.Infrastructure.Providers;

namespace Xap.Infrastructure.Security {
    public class XapSecurityProvider : IXapSecurityProvider {

        private static IXapSecurityProvider instance = null;
        static readonly object padlock = new object();

        public static IXapSecurityProvider Instance {
            get {
                lock (padlock) {
                    if (instance == null) {
                        instance = ProviderLoader.Instance.LoadSecurityProvider();
                    }
                    return instance;
                }
            }
            set { instance = value; }
        }


        #region "XapUser"
        IXapUser IXapSecurityProvider.XapUser {
            get { return null; }
        }

        IXapUser IXapSecurityProvider.InitializeUser(string userName, string password) {
            return null;
        }
        #endregion
    }
}
