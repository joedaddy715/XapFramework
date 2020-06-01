namespace Xap.Evaluation.Factory.Builders {
    public class XapRuleBuilder {
        #region "Constuctors"
        private XapRuleBuilder() { }
        public static XapRuleBuilder Create() {
            return new XapRuleBuilder();
        }
        #endregion

        #region "Properties"
        private IXapRule _rule = XapRule.Create();
        #endregion

        #region "Builder Methods"
        public XapRuleBuilder RuleId(int ruleId) {
            _rule.RuleId = ruleId;
            return this;
        }

        public XapRuleBuilder RuleName(string ruleName) {
            _rule.RuleName = ruleName;
            return this;
        }

        public XapRuleBuilder RuleSyntax(string ruleSyntax) {
            _rule.RuleSyntax = ruleSyntax;
            return this;
        }

        public XapRuleBuilder RuleMessage(string ruleMessage) {
            _rule.RuleMessage = ruleMessage;
            return this;
        }

        public XapRuleBuilder RuleDescription(string ruleDescription) {
            _rule.RuleDescription = ruleDescription;
            return this;
        }

        public XapRuleBuilder RuleType(string ruleType) {
            _rule.RuleType = ruleType;
            return this;
        }

        public XapRuleBuilder RuleValue(string ruleValue) {
            _rule.RuleValue = ruleValue;
            return this;
        }

        public XapRuleBuilder PropertyName(string propertyName) {
            _rule.PropertyName = propertyName;
            return this;
        }

        public XapRuleBuilder PropertyAlias(string propertyAlias) {
            _rule.PropertyAlias = propertyAlias;
            return this;
        }

        public XapRuleBuilder SysLstTrxOpNo(string sysLstTrxOpNo) {
            _rule.SysLstTrxOpNo = sysLstTrxOpNo;
            return this;
        }

        public XapRuleBuilder AddDependent(IXapRuleDependent dependent) {
            _rule.AddDependent(dependent);
            return this;
        }
        public IXapRule Build() {
            return _rule;
        }
        #endregion
    }
}
