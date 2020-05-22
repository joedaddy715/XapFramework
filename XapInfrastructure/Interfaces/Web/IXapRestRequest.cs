namespace Xap.Infrastructure.Interfaces.Web {
    public interface IXapRestRequest {
        string EndPoint { get; }
        IXapRestRequest AddQueryParameter(string name, string value);
        IXapRestRequest AppendEndpoint(string name, string value);
        string GetResponse();
    }
}
