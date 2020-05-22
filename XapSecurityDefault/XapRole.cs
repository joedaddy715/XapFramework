using Xap.Infrastructure.Interfaces.Security;

namespace Xap.Security.Default {
    internal class XapRole:IXapRole {
        private XapRole() { }

        internal static IXapRole Create() {
            return new XapRole();
        }

        private int _roleId;
        int IXapRole.RoleId {
            get => _roleId;
            set => _roleId = value;
        }

        private string _roleName = string.Empty;
        string IXapRole.RoleName {
            get => _roleName;
            set => _roleName = value;
        }

        private string _roleDescription = string.Empty;
        string IXapRole.RoleDescription {
            get => _roleDescription;
            set => _roleDescription = value;
        }

    }
}
