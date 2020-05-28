using System.Data;
using System.Data.SqlClient;
using Xap.Data.Factory.Interfaces;

namespace Xap.Data.MsSql {
    public class Provider : IXapDataConnectionProvider {
        IDbConnection IXapDataConnectionProvider.GetConnection(string connString) {
            IDbConnection conn = new SqlConnection(connString);
            if (conn.State != ConnectionState.Open) {
                conn.Open();
            }
            return conn;
        }
    }
}
