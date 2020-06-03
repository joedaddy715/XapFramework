using System.Collections.Generic;

namespace Xap.Evaluation.Factory.Interfaces {
    public interface IXapRule {
        int RuleId { get; set; }
        string RuleName { get; set; }
        string RuleType { get; set; }
        string RuleDescription { get; set; }
        string RuleSyntax { get; set; }
        string RuleMessage { get; set; }
        string RuleValue { get; set; }
        bool EvaluateRule();
        T EvaluateRule<T>();
        void AddRuleVariable(IXapRuleVariable ruleVariable);
        bool HasRuleVariable(string variableName);
        IEnumerable<IXapRuleVariable> GetRuleVariables();
        string SyntaxError { get; }
    }
}
