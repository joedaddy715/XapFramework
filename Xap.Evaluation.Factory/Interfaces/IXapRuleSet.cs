using System.Collections.Generic;

namespace Xap.Evaluation.Factory.Interfaces {
    public interface IXapRuleSet {
        IXapRule CreateRule(string ruleName);
        IXapRuleSet AddRule(IXapRule rule);
        IEnumerable<IXapRule> GetRules();
        IXapRuleSet RemoveRule(string ruleName);
        void ClearRules();

        int RuleCount { get; }
        string RuleSetName { get; }
    }
}
