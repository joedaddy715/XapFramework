namespace Xap.Data.Factory.Interfaces {
    public interface IXapPocoField {
        string FieldName { get; set; }
        bool DoesInsert { get; set; }
        bool DoesSelect { get; set; }
        bool DoesSelectList { get; set; }
        bool DoesUpdate { get; set; }
        bool DoesDelete { get; set; }
        bool IsIdentity { get; set; }
        string DbColumn { get; set; }
        string DataType { get; set; }
    }
}
