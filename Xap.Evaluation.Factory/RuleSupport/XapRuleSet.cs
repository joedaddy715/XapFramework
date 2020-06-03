using System;
using System.Collections.Generic;
using Xap.Evaluation.Factory.Interfaces;
using Xap.Infrastructure.Caches;
using Xap.Infrastructure.Exceptions;

namespace Xap.Evaluation.Factory.RuleSupport {
    internal class XapRuleSet : IXapRuleSet {
        #region "Constructors"
        private XapRuleSet(string ruleSetName) {
            _ruleSetName = ruleSetName;
        }

        public static IXapRuleSet Create(string ruleSetName) {
            return new XapRuleSet(ruleSetName);
        }
        #endregion

        #region "Properties"
        XapCache<string, IXapRule> rules = new XapCache<string, IXapRule>();

        int IXapRuleSet.RuleCount {
            get => rules.Count;
        }

        private string _ruleSetName = string.Empty;
        string IXapRuleSet.RuleSetName {
            get => _ruleSetName;
        }
        #endregion

        #region "Methods"
        IXapRuleSet IXapRuleSet.AddRule(IXapRule rule) {
            try {
                rules.AddItem(rule.RuleName, rule);
                return this;
            } catch (Exception ex) {
                throw new XapException($"Error adding rule {rule.RuleName}", ex);
            }
        }

        void IXapRuleSet.ClearRules() {
            try {
                rules.ClearCache();
            } catch (Exception ex) {
                throw new XapException($"Error clearing rules", ex);
            }
        }

        IXapRule IXapRuleSet.GetRule(string ruleName) {
            if(rules.GetItem(ruleName) != null) {
                return rules.GetItem(ruleName);
            }
            return XapRule.Create(ruleName);
        }

        IEnumerable<IXapRule> IXapRuleSet.GetRules() {
            foreach (KeyValuePair<string, IXapRule> rule in rules.GetItems()) {
                yield return rule.Value;
            }
        }

        IXapRuleSet IXapRuleSet.RemoveRule(string ruleName) {
            try {
                rules.RemoveItem(ruleName);
                return this;
            } catch (Exception ex) {
                throw new XapException($"Error removing rule {ruleName}", ex);
            }
        }
        #endregion
    }
}
