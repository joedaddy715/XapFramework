﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Xap.Validation.Factory.Services {
    public class ValidationService : IXapValidationService {
        #region "Constructors"
        private ValidationService() { }

        public static IXapValidationService Create() {
            return new ValidationService();
        }
        #endregion

        #region "Properties"
        private XapCache<string, IXapRuleSet> ruleSets = new XapCache<string, IXapRuleSet>();
        private XapBrokenRules brokenRules = null;

        int IXapValidationService.RuleSetCount {
            get => ruleSets.Count;
        }

        private bool _breakOnError = false;
        bool IXapValidationService.BreakOnError {
            get => _breakOnError;
            set => _breakOnError = value;
        }

        private IXapSecureObject _secureObject = null;
        IXapSecureObject IXapValidationService.SecureObject {
            get => _secureObject;
        }
        #endregion

        #region "Methods"
        IXapRuleSet IXapValidationService.AddRuleSet(string ruleSetName) {
            IXapRuleSet ruleSet = null;
            try {
                ruleSet = ruleSets.GetItem(ruleSetName);
                if (ruleSet == null) {
                    ruleSet = XapRuleSet.Create(ruleSetName);
                }
                ruleSets.AddItem(ruleSet.RuleSetName, ruleSet);
                return ruleSet;
            } catch (Exception ex) {
                throw new XapException($"Error adding rule set {ruleSet.RuleSetName}", ex);
            }
        }

        IEnumerable<IXapBrokenRule> IXapValidationService.BrokenRules() {
            foreach (IXapBrokenRule brokenRule in brokenRules.GetBrokenRules()) {
                yield return brokenRule;
            }
        }
        int IXapValidationService.BrokenRulesCount {
            get => brokenRules.Count;
        }

        void IXapValidationService.ClearRuleSets() {
            try {
                ruleSets.ClearCache();
            } catch (Exception ex) {
                throw new XapException($"Error clearing rule sets", ex);
            }
        }

        IXapValidationService IXapValidationService.EvaluateValidationRuleSet(string ruleSetName) {
            try {
                IXapRuleSet ruleSet = ruleSets.GetItem(ruleSetName);
                brokenRules = XapBrokenRules.Create();

                if (ruleSet != null) {
                    foreach (IXapRule rule in ruleSet.GetRules()) {
                        bool result = rule.EvaluateRule<bool>();
                        if (result == false) {
                            brokenRules.AddBrokenRule(XapBrokenRule.Create(rule.PropertyName, rule.RuleName, rule.RuleMessage));
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

        IXapValidationService IXapValidationService.EvaluateValidationRuleSets() {
            try {
                brokenRules = XapBrokenRules.Create();
                foreach (KeyValuePair<string, IXapRuleSet> ruleSet in ruleSets.GetItems()) {
                    foreach (IXapRule rule in ruleSet.Value.GetRules()) {
                        bool result = rule.EvaluateRule<bool>();
                        if (result == false) {
                            brokenRules.AddBrokenRule(XapBrokenRule.Create(rule.PropertyName, rule.RuleName, rule.RuleMessage));
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

        IXapValidationService IXapValidationService.EvaluateSecurityRuleSet(string ruleSetName) {
            try {
                IXapRuleSet ruleSet = ruleSets.GetItem(ruleSetName);
                _secureObject = XapSecureObject.Create();

                if (ruleSet != null) {
                    foreach (IXapRule rule in ruleSet.GetRules()) {
                        bool result = rule.EvaluateRule<bool>();
                        if (result == false) {
                            if (rule.PropertyName == "CanInsert") {
                                _secureObject.CanInsert = false;
                            } else if (rule.PropertyName == "CanSelect") {
                                _secureObject.CanSelect = false;
                            } else if (rule.PropertyName == "CanUpdate") {
                                _secureObject.CanUpdate = false;
                            } else if (rule.PropertyName == "CanDelete") {
                                _secureObject.CanDelete = false;
                            } else {
                                _secureObject.AddSecureProperty(rule.PropertyName);
                            }

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

        IXapValidationService IXapValidationService.EvaluateSecurityRuleSets() {
            try {
                _secureObject = XapSecureObject.Create();
                foreach (KeyValuePair<string, IXapRuleSet> ruleSet in ruleSets.GetItems()) {
                    foreach (IXapRule rule in ruleSet.Value.GetRules()) {
                        bool result = rule.EvaluateRule<bool>();
                        if (result == false) {
                            if (rule.PropertyName == "CanInsert") {
                                _secureObject.CanInsert = false;
                            } else if (rule.PropertyName == "CanSelect") {
                                _secureObject.CanSelect = false;
                            } else if (rule.PropertyName == "CanUpdate") {
                                _secureObject.CanUpdate = false;
                            } else if (rule.PropertyName == "CanDelete") {
                                _secureObject.CanDelete = false;
                            } else {
                                _secureObject.AddSecureProperty(rule.PropertyName);
                            }

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

        IXapRuleSet IXapValidationService.GetRuleSet(string ruleSetName) {
            try {
                return ruleSets.GetItem(ruleSetName);
            } catch (Exception ex) {
                throw new XapException($"Error retrieving rule set {ruleSetName}", ex);
            }
        }

        IEnumerable<IXapRuleSet> IXapValidationService.GetRuleSets() {
            foreach (KeyValuePair<string, IXapRuleSet> kvp in ruleSets.GetItems()) {
                yield return kvp.Value;
            }
        }

        IXapValidationService IXapValidationService.RemoveRuleSet(string ruleSetName) {
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
