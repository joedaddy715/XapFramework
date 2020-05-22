using System.Data;
using Xap.Data.Factory.Interfaces;

namespace Xap.Data.Factory {
    public abstract class DbConnectionProvider : IXapDataConnectionProvider{
        public DbConnectionProvider() {

        }

        public IDbConnection GetConnection(string connString) {
            return SetConnection(connString);
        }

        protected virtual IDbConnection SetConnection(string connString) {
            return null;
        }
    }
}
