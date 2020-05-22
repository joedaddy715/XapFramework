namespace Xap.Data.Factory.Interfaces {
    public interface IXapPocoMap {
        string ObjectName { get; set; }
        string SelectProcedure { get; set; }
        string SelectListProcedure { get; set; }
        string InsertProcedure { get; set; }
        string DeleteProcedure { get; set; }
        string UpdateProcedure { get; set; }
        IXapPocoField GetField(string fieldName);
        string GetIdentityField();
        void AddField(IXapPocoField pocoField);
    }
}
