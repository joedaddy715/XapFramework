namespace Xap.Password.Factory.Interfaces {
    public interface IXapPasswordProvider{
        IXapPasswordContext RetrievePassword(IXapPasswordContext passwordContext);
    }
}
