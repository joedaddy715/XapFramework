using System.Collections.Generic;

namespace Xap.Infrastructure.Interfaces.Security {
    public interface IXapSecureObject {
        bool CanSelect { get; set; }
        bool CanInsert { get; set; }
        bool CanUpdate { get; set; }
        bool CanDelete { get; set; }

        IXapSecureObject AddSecureProperty(string propertyName);

        IEnumerable<string> SecuredProperties();
    }
}
