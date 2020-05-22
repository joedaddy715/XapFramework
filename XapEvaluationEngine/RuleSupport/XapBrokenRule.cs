using Xap.Infrastructure.Interfaces.Evaluation;

namespace Xap.Evaluation.Engine.RuleSupport {
    internal class XapBrokenRule : IXapBrokenRule {
        #region "Constructors"
        private XapBrokenRule() { }
        private XapBrokenRule(string propertyName,string ruleName,string ruleMessage) {
            _propertyName = propertyName;
            _ruleName = ruleName;
            _ruleMessage = ruleMessage;
        }

        public static XapBrokenRule Create() {
            return new XapBrokenRule();
        }

        public static XapBrokenRule Create(string propertyName, string ruleName, string ruleMessage) {
            return new XapBrokenRule(propertyName, ruleName, ruleMessage);
        }
        #endregion

        #region "interface properties"
        private string _propertyName = string.Empty;
        string IXapBrokenRule.PropertyName {
            get => _propertyName;
            set => _propertyName = value;
        }

        private string _ruleName = string.Empty;
        string IXapBrokenRule.RuleName {
            get => _ruleName;
            set => _ruleName = value;
        }

        private string _ruleMessage = string.Empty;
        string IXapBrokenRule.RuleMessage {
            get => _ruleMessage;
            set => _ruleMessage = value;
        }
        #endregion
    }
}
