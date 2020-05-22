namespace Xap.Infrastructure.Interfaces.Evaluation {
    public interface IXapBrokenRule {
        string PropertyName { get; set; }
        string RuleName { get; set; }
        string RuleMessage { get; set; }
    }
}
