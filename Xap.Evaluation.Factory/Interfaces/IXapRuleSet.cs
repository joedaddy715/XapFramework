using System.Collections.Generic;

namespace Xap.Evaluation.Factory.Interfaces {
    public interface IXapRuleSet {
        IXapRuleSet AddRule(IXapRule rule);
        IXapRule GetRule(string ruleName);
        IEnumerable<IXapRule> GetRules();
        IXapRuleSet RemoveRule(string ruleName);
        void ClearRules();

        int RuleCount { get; }
        string RuleSetName { get; }
    }
}
