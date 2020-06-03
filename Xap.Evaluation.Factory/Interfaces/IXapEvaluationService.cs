using System.Collections.Generic;

namespace Xap.Evaluation.Factory.Interfaces {
    //TODO: Replace IXapValidationService
    public interface IXapEvaluationService{
        void AddRuleSet(IXapRuleSet ruleSet);
        IXapRuleSet GetRuleSet(string ruleSetName);
        IEnumerable<IXapRuleSet> GetRuleSets();
        IXapEvaluationService EvaluateRuleSet(string ruleSetName);
        IXapEvaluationService EvaluateRuleSets();
        IEnumerable<IXapBrokenRule> BrokenRules();
        IXapEvaluationService RemoveRuleSet(string ruleSetName);
        void ClearRuleSets();
        int BrokenRulesCount { get; }
        int RuleSetCount { get; }
        bool BreakOnError { get; set; }
    }
}
