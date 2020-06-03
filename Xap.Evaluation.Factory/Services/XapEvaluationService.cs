using System;
using System.Collections.Generic;
using Xap.Evaluation.Factory.Interfaces;
using Xap.Evaluation.Factory.RuleSupport;
using Xap.Infrastructure.Caches;
using Xap.Infrastructure.Exceptions;

namespace Xap.Evaluation.Factory.Services {
    public class XapEvaluationService :IXapEvaluationService {
        #region "Constructors"
        public XapEvaluationService() { }
        public static IXapEvaluationService Create() {
            return new XapEvaluationService();
        }
        #endregion

        #region "private properties"
        private XapCache<string, IXapRuleSet> ruleSets = new XapCache<string, IXapRuleSet>();
        private XapBrokenRules brokenRules = null;
        #endregion

        #region "interface properties"
        int IXapEvaluationService.RuleSetCount {
            get => ruleSets.Count;
        }

        private bool _breakOnError = false;
        bool IXapEvaluationService.BreakOnError {
            get => _breakOnError;
            set => _breakOnError = value;
        }
        #endregion

        #region "interface methods"
        void IXapEvaluationService.AddRuleSet(IXapRuleSet ruleSet) {
            try {
                    ruleSets.AddItem(ruleSet.RuleSetName, ruleSet);
            } catch (Exception ex) {
                throw new XapException($"Error adding rule set {ruleSet.RuleSetName}", ex);
            }
        }

        IEnumerable<IXapBrokenRule> IXapEvaluationService.BrokenRules() {
            foreach (IXapBrokenRule brokenRule in brokenRules.GetBrokenRules()) {
                yield return brokenRule;
            }
        }
        int IXapEvaluationService.BrokenRulesCount {
            get => brokenRules.Count;
        }

        void IXapEvaluationService.ClearRuleSets() {
            try {
                ruleSets.ClearCache();
            } catch (Exception ex) {
                throw new XapException($"Error clearing rule sets", ex);
            }
        }

        IXapEvaluationService IXapEvaluationService.EvaluateRuleSet(string ruleSetName) {
            try {
                IXapRuleSet ruleSet = ruleSets.GetItem(ruleSetName);
                brokenRules = XapBrokenRules.Create();

                if (ruleSet != null) {
                    foreach (IXapRule rule in ruleSet.GetRules()) {
                        bool result = rule.EvaluateRule<bool>();
                        if (result == false) {
                            brokenRules.AddBrokenRule(XapBrokenRule.Create(ruleSetName, rule.RuleName, rule.RuleMessage));
                            if (_breakOnError) {
                                break;
                            }
                        }
                    }
                }
                return this;
            } catch (Exception ex) {
                throw new XapException($"Error evaluating rule set {ruleSetName}", ex);
            }
        }

        IXapEvaluationService IXapEvaluationService.EvaluateRuleSets() {
            try {
                brokenRules = XapBrokenRules.Create();
                foreach (KeyValuePair<string, IXapRuleSet> ruleSet in ruleSets.GetItems()) {
                    foreach (IXapRule rule in ruleSet.Value.GetRules()) {
                        bool result = rule.EvaluateRule<bool>();
                        if (result == false) {
                            brokenRules.AddBrokenRule(XapBrokenRule.Create(ruleSet.Key, rule.RuleName, rule.RuleMessage));
                            if (_breakOnError) {
                                break;
                            }
                        }
                    }
                    if (_breakOnError) {
                        break;
                    }
                }
                return this;
            } catch (Exception ex) {
                throw new XapException($"Error evaluating rule sets", ex);
            }
        }

        IXapRuleSet IXapEvaluationService.GetRuleSet(string ruleSetName) {
            try {
                if (ruleSets.GetItem(ruleSetName) != null) {
                    return ruleSets.GetItem(ruleSetName);
                }
                return XapRuleSet.Create(ruleSetName);
            } catch (Exception ex) {
                throw new XapException($"Error retrieving rule set {ruleSetName}", ex);
            }
        }

        IEnumerable<IXapRuleSet> IXapEvaluationService.GetRuleSets() {
            foreach (KeyValuePair<string, IXapRuleSet> kvp in ruleSets.GetItems()) {
                yield return kvp.Value;
            }
        }

        IXapEvaluationService IXapEvaluationService.RemoveRuleSet(string ruleSetName) {
            try {
                ruleSets.RemoveItem(ruleSetName);
                return this;
            } catch (Exception ex) {
                throw new XapException($"Error removing rule set {ruleSetName}", ex);
            }
        }
        #endregion
    }
}
