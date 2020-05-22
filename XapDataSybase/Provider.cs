using System.Data;
using Xap.Data.Factory.Interfaces;

namespace Xap.Data.Sybase {
    public class Provider : IXapDataConnectionProvider {
        IDbConnection IXapDataConnectionProvider.GetConnection(string connString) {
            //IDbConnection conn = new AseConnection(connString);
            //if (conn.State != ConnectionState.Open) {
            //    conn.Open();
            //}
            //return conn;
            return null;
        }
    }
}
