using System.Data;

namespace Xap.Data.Factory.Interfaces {
    public interface IXapDataConnectionProvider {
        IDbConnection GetConnection(string connString);
    }
}
