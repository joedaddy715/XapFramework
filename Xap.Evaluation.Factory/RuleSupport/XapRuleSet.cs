﻿using System;
using System.Collections.Generic;
using System.Linq;

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
        IXapRule IXapRuleSet.CreateRule(string ruleName) {
            return XapRule.Create(ruleName);
        }

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