using System;
using System.Collections.Generic;
using Xap.Infrastructure.Caches;
using Xap.Infrastructure.Exceptions;
using Xap.Infrastructure.Interfaces.Evaluation;
using Xap.Infrastructure.Logging;

namespace Xap.Evaluation.Engine.RuleSupport {
    internal class XapRuleSet:IXapRuleSet {
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
        IXapRule IXapRuleSet.CreateRule(string ruleName) {
            return XapRule.Create(ruleName);
        }

        IXapRuleSet IXapRuleSet.AddRule(IXapRule rule) {
            try {
                rules.AddItem(rule.RuleName, rule);
                return this;
            } catch (Exception ex) {
                XapLogger.Instance.Error(ex.Message);
                throw new XapException($"Error adding rule {rule.RuleName}");
            }
        }

        void IXapRuleSet.ClearRules() {
            try {
                rules.ClearCache();
            } catch (Exception ex) {
                XapLogger.Instance.Error(ex.Message);
                throw new XapException($"Error clearing rules");
            }
        }

        IEnumerable<IXapRule> IXapRuleSet.GetRules() {
           foreach(KeyValuePair<string,IXapRule> rule in rules.GetItems()) {
                yield return rule.Value;
            }
        }

        IXapRuleSet IXapRuleSet.RemoveRule(string ruleName) {
            try {
                rules.RemoveItem(ruleName);
                return this;
            } catch (Exception ex) {
                XapLogger.Instance.Error(ex.Message);
                throw new XapException($"Error removing rule {ruleName}");
            }
        }
        #endregion
    }
}
