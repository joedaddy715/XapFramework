using Xap.Evaluation.Engine.RuleSupport;
using Xap.Infrastructure.Core;
using Xap.Infrastructure.Interfaces.Evaluation;

namespace Xap.Evaluation.Engine.Builders {
    public class XapRuleSearchBuilder {
        #region "Constructors"
        private XapRuleSearchBuilder() { }
        public static XapRuleSearchBuilder Create() {
            return new XapRuleSearchBuilder();
        }
        #endregion

        #region "Properties"
        private IXapRuleSearch ruleSearch = XapRuleSearch.Create();
        #endregion

        #region "Builder Methods"
        public XapRuleSearchBuilder SourceObject(XapObjectCore sourceObject ) {
            ruleSearch.SourceObject = sourceObject;
            return this;
        }

        public XapRuleSearchBuilder RuleType(string ruleType) {
            ruleSearch.RuleType = ruleType;
            return this;
        }

        public XapRuleSearchBuilder NameSpace(string nameSpace) {
            ruleSearch.NameSpace = nameSpace;
            return this;
        }

        public XapRuleSearchBuilder PropertyName(string propertyName) {
            ruleSearch.PropertyName = propertyName;
            return this;
        }

        public XapRuleSearchBuilder LobName(string lobName) {
            ruleSearch.LobName = lobName;
            return this;
        }

        public XapRuleSearchBuilder ComponentName(string componentName) {
            ruleSearch.ComponentName = componentName;
            return this;
        }
        public IXapRuleSearch Build() {
            return ruleSearch;
        }
        #endregion
    }
}
