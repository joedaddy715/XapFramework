using System.Collections.Generic;
using Xap.Evaluation.Factory.Interfaces;
using Xap.Infrastructure.Caches;

namespace Xap.Evaluation.Factory.RuleSupport {
    internal class XapBrokenRules {
        #region "Constructors"
        internal XapBrokenRules() { }
        internal static XapBrokenRules Create() {
            return new XapBrokenRules();
        }
        #endregion

        #region "Properties"
        private XapCache<string, IXapBrokenRule> _brokenRules = new XapCache<string, IXapBrokenRule>();
        #endregion

        #region "Public Methods"
        internal IEnumerable<IXapBrokenRule> GetBrokenRules() {
            foreach (KeyValuePair<string, IXapBrokenRule> kvp in _brokenRules.GetItems()) {
                yield return kvp.Value;
            }
        }

        internal XapBrokenRules AddBrokenRule(IXapBrokenRule brokenRule) {
            _brokenRules.AddItem((_brokenRules.Count + 1).ToString(), brokenRule);
            return this;
        }

        internal void Clear() {
            _brokenRules.ClearCache();
        }

        internal int Count {
            get => _brokenRules.Count;
        }
        #endregion
    }
}
