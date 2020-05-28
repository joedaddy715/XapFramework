//using IBM.Data.DB2;
using System.Data;
using Xap.Data.Factory.Interfaces;

namespace Xap.Data.Db2 {
    public class Provider : IXapDataConnectionProvider {
        IDbConnection IXapDataConnectionProvider.GetConnection(string connString) {
            //IDbConnection conn = new DB2Connection(connString);
            //if (conn.State != ConnectionState.Open) {
            //    conn.Open();
            //}
            //return conn;
            return null;
        }
    }
}
