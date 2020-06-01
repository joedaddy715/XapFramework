namespace Xap.Evaluation.Factory.Interfaces {
    public interface IXapBrokenRule {
        string PropertyName { get; set; }
        string RuleName { get; set; }
        string RuleMessage { get; set; }
    }
}
