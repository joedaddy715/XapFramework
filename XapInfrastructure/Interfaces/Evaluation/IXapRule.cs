using System.Collections.Generic;

namespace Xap.Infrastructure.Interfaces.Evaluation {
    public interface IXapRule {
        int RuleId { get; set; }
        string RuleName { get; set; }
        string RuleType { get; set; }
        string RuleDescription { get; set; }
        string RuleSyntax { get; set; }
        string RuleMessage { get; set; }
        string PropertyName { get; set; }
        string PropertyAlias { get; set; }
        string RuleValue { get; set; }
        bool EvaluateRule();
        T EvaluateRule<T>();
        void AddDependent(IXapRuleDependent dependency);
        bool HasDependent(string dependentName);
        IEnumerable<IXapRuleDependent> GetDependents();
        string SysLstTrxOpNo { get; set; }
        string SyntaxError { get; }
    }
}
