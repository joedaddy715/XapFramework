using Xap.Infrastructure.Core;
using Xap.Infrastructure.Interfaces.Evaluation;

namespace Xap.Infrastructure.Evaluation.RuleSupport {
    internal class XapRuleSearch:IXapRuleSearch {
        #region "Constructors"
        private XapRuleSearch() { }

        public static XapRuleSearch Create() {
            return new XapRuleSearch();
        }
        #endregion

        private XapObjectCore _sourceObject = null;
        XapObjectCore IXapRuleSearch.SourceObject {
            get => _sourceObject;
            set => _sourceObject = value;
        }

        private string _ruleType = string.Empty;
        string IXapRuleSearch.RuleType {
            get => _ruleType;
            set => _ruleType = value;
        }

        private string _nameSpace = string.Empty;
        string IXapRuleSearch.NameSpace {
            get => _nameSpace;
            set => _nameSpace = value;
        }

        private string _propertyName = string.Empty;
        string IXapRuleSearch.PropertyName {
            get => _propertyName;
            set => _propertyName = value;
        }

        private string _lobName = string.Empty;
        string IXapRuleSearch.LobName {
            get => _lobName;
            set => _lobName = value;
        }

        private string _componentName = string.Empty;
        string IXapRuleSearch.ComponentName {
            get => _componentName;
            set => _componentName = value;
        }
    }
}
