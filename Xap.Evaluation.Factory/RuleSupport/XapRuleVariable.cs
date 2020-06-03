using Xap.Evaluation.Factory.Interfaces;

namespace Xap.Evaluation.Factory.RuleSupport {
    internal class XapRuleVariable : IXapRuleVariable {
        private XapRuleVariable(string variableName) {
            _variableName = variableName;
        }

        internal static XapRuleVariable Create(string variableName) {
            return new XapRuleVariable(variableName);
        }

        private XapRuleVariable(string variableName, string variableValue) {
            _variableName = variableName;
            _variableValue = variableValue;
        }

        internal static XapRuleVariable Create(string variableName, string variableValue) {
            return new XapRuleVariable(variableName, variableValue);
        }

        private XapRuleVariable(string variableName, string variableAlias,string variableValue) {
            _variableName = variableName;
            _variableAlias = variableAlias;
            _variableValue = variableValue;
        }

        internal static XapRuleVariable Create(string variableName, string variableAlias,string variableValue) {
            return new XapRuleVariable(variableName, variableAlias,variableValue);
        }

        private string _variableName = string.Empty;
        string IXapRuleVariable.VariableName {
            get => _variableName;
        }

        private string _variableAlias = string.Empty;
        string IXapRuleVariable.VariableAlias {
            get => _variableAlias;
            set => _variableAlias = value;
        }

        private string _variableValue = string.Empty;
        string IXapRuleVariable.VariableValue {
            get => _variableValue;
            set => _variableValue = value;
        }
    }
}
