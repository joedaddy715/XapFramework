using Xap.Infrastructure.Interfaces.Security;

namespace Xap.Security.Default {
    public class Provider : IXapSecurityProvider {
        #region "Constructors"
        public Provider() {

        }
        #endregion
        private IXapUser _xapUser =null;
        IXapUser IXapSecurityProvider.XapUser {
            get { return _xapUser; }
        }

        IXapUser IXapSecurityProvider.InitializeUser(string userName, string password) {
            _xapUser = XapUser.Create(userName, password);
            return _xapUser;
        }
    }
}
