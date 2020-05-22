using Xap.Evaluation.Engine.RuleSupport;
using Xap.Infrastructure.Interfaces.Evaluation;

namespace Xap.Evaluation.Engine.Builders {
    public class XapBrokenRuleBuilder {
        #region "Constructors"
        private XapBrokenRuleBuilder() { }
        public static XapBrokenRuleBuilder Create() {
            return new XapBrokenRuleBuilder();
        }
        #endregion

        #region "Properties"
        private IXapBrokenRule _brokenRule = XapBrokenRule.Create();
        #endregion

        #region "builder methods"
        public XapBrokenRuleBuilder PropertyName(string propertyName) {
            _brokenRule.PropertyName = propertyName;
            return this;
        }

        public XapBrokenRuleBuilder RuleName(string ruleName) {
            _brokenRule.RuleName = ruleName;
            return this;
        }

        public XapBrokenRuleBuilder RuleMessage(string ruleMessage) {
            _brokenRule.RuleMessage = ruleMessage;
            return this;
        }

        public IXapBrokenRule Build() {
            return _brokenRule;
        }
        #endregion
    }
}
