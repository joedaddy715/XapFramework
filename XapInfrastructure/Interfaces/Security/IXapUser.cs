using System.Collections.Generic;

namespace Xap.Infrastructure.Interfaces.Security {
    public interface IXapUser {
        #region "Properties"
        int UserId { get; set; }
        string UserName { get; set; }
        string Password { get; set; }
        string FirstName { get; set; }
        string LastName { get; set; }
        string FullName { get; set; }
        string FullNameReverse { get; set; }
        bool IsAuthenticated { get; set; }
        string EmailAddress { get; set; }
        bool IsActive { get; set; }
        string CurrentLob { get; }
       // IEnumerable<IXapGenericData> UserRoles();
       // IEnumerable<IXapGenericData> UserLobs();
        #endregion

        #region "Methods"
        void Login(string lobName);
        void LogOut();
        string UserRolesList { get; }
        #endregion
    }
}
