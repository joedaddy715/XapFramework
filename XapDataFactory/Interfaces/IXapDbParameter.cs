using System.Data;

namespace Xap.Data.Factory.Interfaces {
    public interface IXapDbParameter {
        string ParameterName { get; set; }
        object ParameterValue { get; set; }
        ParameterDirection ParameterDirection { get; set; }
    }
}
