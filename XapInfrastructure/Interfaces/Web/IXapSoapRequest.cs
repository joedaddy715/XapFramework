using System.IO;
using System.Xml;

namespace Xap.Infrastructure.Interfaces.Web {
    public interface IXapSoapRequest {
        string EndPoint { get;  }
        string EnvelopeName { get;  }
        IXapSoapRequest AddHeadParameter(string name, string value);
        IXapSoapRequest AddBodyParameter(string name, string value);
        XmlDocument ExecuteXmlRequest();
        Stream ExecuteMtomRequest();
    }
}
