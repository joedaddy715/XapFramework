namespace Xap.Evaluation.Factory.Interfaces {
    public interface IXapRuleVariable  {
        string VariableName { get; }
        string VariableAlias { get; set; }
        string VariableValue { get; set; }
    }
}
