namespace Xap.Evaluation.Factory.Interfaces {
    public interface IXapRuleSearch {
        XapObjectCore SourceObject { get; set; }
        string RuleType { get; set; }
        string NameSpace { get; set; }
        string PropertyName { get; set; }
        string LobName { get; set; }
        string ComponentName { get; set; }
    }
}
