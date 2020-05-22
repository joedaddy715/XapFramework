using System.Data;
using System.Data.Odbc;
using Xap.Data.Factory.Interfaces;

namespace Xap.Data.Odbc {
    public class Provider : IXapDataConnectionProvider {
        IDbConnection IXapDataConnectionProvider.GetConnection(string connString) {
            IDbConnection conn = new OdbcConnection(connString);
            if (conn.State != ConnectionState.Open) {
                conn.Open();
            }
            return conn;
        }
    }
}
