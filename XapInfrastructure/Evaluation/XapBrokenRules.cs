using System.Collections.Generic;
using Xap.Infrastructure.Caches;
using Xap.Infrastructure.Interfaces.Evaluation;

namespace Xap.Infrastructure.Evaluation {
    public class XapBrokenRules {
        #region "Constructors"
        private XapBrokenRules() { }
        public static XapBrokenRules Create() {
            return new XapBrokenRules();
        }
        #endregion

        #region "Properties"
        private XapCache<string, IXapBrokenRule> _brokenRules = new XapCache<string, IXapBrokenRule>();
        #endregion

        #region "Public Methods"
        public IEnumerable<IXapBrokenRule> GetBrokenRules() {
            foreach (KeyValuePair<string, IXapBrokenRule> kvp in _brokenRules.GetItems()) {
                yield return kvp.Value; ;
            }
        }

        public XapBrokenRules AddBrokenRule(IXapBrokenRule brokenRule) {
            _brokenRules.AddItem(brokenRule.RuleName, brokenRule);
            return this;
        }

        public void Clear() {
            _brokenRules.ClearCache();
        }

        public int Count {
            get => _brokenRules.Count;
        }
        #endregion
    }
}
