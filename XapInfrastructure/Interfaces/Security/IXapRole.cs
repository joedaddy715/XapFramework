namespace Xap.Infrastructure.Interfaces.Security {
    public interface IXapRole {
        int RoleId { get; set; }
        string RoleName { get; set; }
        string RoleDescription { get; set; }
    }
}
