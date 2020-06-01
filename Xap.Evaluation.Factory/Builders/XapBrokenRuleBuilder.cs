using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xap.Evaluation.Factory.Builders
{
    internal class XapBrokenRuleBuilder {
        #region "Constructors"
        private XapBrokenRuleBuilder() { }
        internal static XapBrokenRuleBuilder Create() {
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
