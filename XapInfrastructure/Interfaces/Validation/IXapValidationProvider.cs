using Xap.Infrastructure.Interfaces.Security;

namespace Xap.Infrastructure.Interfaces.Validation {
    public interface IXapValidationProvider {
        IXapValidationService LoadRules<T>(T obj, IXapValidationService validationService,IXapUser xapUser,string ruleType);
        IXapValidationService LoadRules<T>(T obj,string propertyName, IXapValidationService validationService,IXapUser user,string ruleType);

    }
}
