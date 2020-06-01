using System.Collections.Generic;
using System.Linq;

namespace Xap.Evaluation.Factory.Cache {
    public class XapRuleCache {
        #region "Constructors"
        private XapRuleCache() { }

        public static XapRuleCache Create() {
            return new XapRuleCache();
        }
        #endregion

        #region "Properties"
        private XapCache<string, IXapRule> _rules = new XapCache<string, IXapRule>();
        #endregion

        #region "Public Methods"
        public IEnumerable<IXapRule> GetRules() {
            foreach (KeyValuePair<string, IXapRule> kvp in _rules.GetItems()) {
                yield return kvp.Value; ;
            }
        }

        public XapRuleCache AddRule(IXapRule rule) {
            _rules.AddItem(rule.RuleName, rule);
            return this;
        }

        public XapRuleCache AddRule(string ruleName, IXapRule rule) {
            _rules.AddItem(ruleName, rule);
            return this;
        }

        public IXapRule GetRule(string ruleName) {
            return _rules.GetItem(ruleName);
        }

        public void Clear() {
            _rules.ClearCache();
        }

        public int Count {
            get => _rules.Count;
        }
        #endregion


    }
}
