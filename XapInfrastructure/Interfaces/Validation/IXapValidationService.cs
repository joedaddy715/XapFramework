using System.Collections.Generic;
using Xap.Infrastructure.Interfaces.Evaluation;
using Xap.Infrastructure.Interfaces.Security;

namespace Xap.Infrastructure.Interfaces.Validation {
    public interface IXapValidationService {
        IXapRuleSet AddRuleSet(string ruleSetName);
        IXapRuleSet GetRuleSet(string ruleSetName);
        IEnumerable<IXapRuleSet> GetRuleSets();
        IXapValidationService EvaluateValidationRuleSet(string ruleSetName);
        IXapValidationService EvaluateValidationRuleSets();
        IXapValidationService EvaluateSecurityRuleSet(string ruleSetName);
        IXapValidationService EvaluateSecurityRuleSets();
        IEnumerable<IXapBrokenRule> BrokenRules();
        IXapSecureObject SecureObject { get; }
        IXapValidationService RemoveRuleSet(string ruleSetName);
        void ClearRuleSets();
        int BrokenRulesCount { get; }

        int RuleSetCount { get; }
        bool BreakOnError { get; set; }
    }
}
