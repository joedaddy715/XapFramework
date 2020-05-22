using System.Data;
using System.Threading.Tasks;
using System.Xml;

namespace Xap.Data.Factory.Interfaces {
    public interface IXapDataProvider {
        IXapDataProvider AddParameter(IXapDbParameter param);
        int ExecuteNonQuery();
        Task<int> ExecuteNonQueryAsync();
        XapDataReader ExecuteReader();
        Task<XapDataReader> ExecuteReaderAsync();
        object ExecuteScalar();
        Task<object> ExecuteScalarAsync();
        DataTable ExecuteDataTable();
        Task<DataTable> ExecuteDataTableAsync();
        DataSet ExecuteDataSet();
        Task<DataSet> ExecuteDataSetAsync();
        XmlDocument ExecuteXml();
        Task<XmlDocument> ExecuteXmlAsync();
        void ExecuteCsv(string filePath);
        Task ExecuteCsvAsync(string filePath);
        string ExecuteJson();
        Task<string> ExecuteJsonAsync();
        void CloseConnection();
        bool IsConnectionValid();
    }
}
