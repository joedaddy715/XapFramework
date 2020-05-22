using System;
using System.Collections.Generic;
using Xap.Evaluation.Engine.Evaluate;
using Xap.Evaluation.Engine.Parser;
using Xap.Infrastructure.Caches;
using Xap.Infrastructure.Extensions;
using Xap.Infrastructure.Interfaces.Evaluation;
using Xap.Infrastructure.Logging;

namespace Xap.Evaluation.Engine.RuleSupport {
    [Serializable]
    internal class XapRule : IXapRule {
        #region "Constructors"
        private XapRule() { }

        private XapRule(string ruleName) {
            _ruleName = ruleName;
        }

        public static IXapRule Create() {
            return new XapRule();
        }

        public static IXapRule Create(string ruleName) {
            return new XapRule(ruleName);
        }
        #endregion

        #region "Properties"
        private XapCache<string, IXapRuleDependent> _dependents = new XapCache<string, IXapRuleDependent>();
        #endregion

        #region "interface"
        private int _ruleId;
        int IXapRule.RuleId {
            get => _ruleId;
            set => _ruleId = value;
        }
        private string _ruleName = string.Empty;
        string IXapRule.RuleName {
            get => _ruleName;
            set => _ruleName = value;
        }


        private string _propertyAlias = string.Empty;
        string IXapRule.PropertyAlias {
            get => _propertyAlias;
            set => _propertyAlias = value;
        }

        private string _ruleType = string.Empty;
        string IXapRule.RuleType {
            get => _ruleType;
            set => _ruleType = value;
        }


        private string _ruleDescription = string.Empty;
        string IXapRule.RuleDescription {
            get => _ruleDescription;
            set => _ruleDescription = value;
        }


        private string _ruleSyntax = string.Empty;
        string IXapRule.RuleSyntax {
            get => _ruleSyntax;
            set => _ruleSyntax = value;
        }


        private string _ruleMessage = string.Empty;
        string IXapRule.RuleMessage {
            get => _ruleMessage;
            set => _ruleMessage = value;
        }


        private string _propertyName = string.Empty;
        string IXapRule.PropertyName {
            get => _propertyName;
            set => _propertyName = value;
        }

        private string _ruleValue = string.Empty;
        string IXapRule.RuleValue {
            get => _ruleValue;
            set => _ruleValue = value;
        }

        private string _sysLstTrxOpNo = string.Empty;
        string IXapRule.SysLstTrxOpNo {
            get => _sysLstTrxOpNo;
            set => _sysLstTrxOpNo = value;
        }

        private string _syntaxError = string.Empty;
        string IXapRule.SyntaxError {
            get => _syntaxError;
        }
        #endregion

        #region "interface methods"
        bool IXapRule.EvaluateRule() {
            try {
                Token token = new Token(_ruleSyntax);
                Evaluator eval = new Evaluator(token);

                if(!eval.Evaluate(out _ruleValue, out _syntaxError)) {
                    throw new Exception(_syntaxError);
                }
                return _ruleValue.ConvertValue<bool>();
            } catch (Exception ex) {
                XapLogger.Instance.Error($"Error evaluating rule {_ruleName}");
                XapLogger.Instance.Write(ex.Message);
                throw;
            }
        }

        T IXapRule.EvaluateRule<T>() {
            try {
                Token token = new Token(_ruleSyntax);
                Evaluator eval = new Evaluator(token);

                if (!eval.Evaluate(out _ruleValue, out _syntaxError)) {
                    throw new Exception(_syntaxError);
                }

                return _ruleValue.ConvertValue<T>();
            } catch (Exception ex) {
                XapLogger.Instance.Error($"Error evaluating rule {_ruleName}");
                XapLogger.Instance.Write(ex.Message);
                throw;
            }
        }

        IEnumerable<IXapRuleDependent> IXapRule.GetDependents() {
            foreach (KeyValuePair<string, IXapRuleDependent> kvp in _dependents.GetItems()) {
                yield return kvp.Value;
            }
        }

        void IXapRule.AddDependent(IXapRuleDependent dependent) {
            _dependents.AddItem(dependent.DependentName, dependent);
        }

        bool IXapRule.HasDependent(string dependentName) {
            if (_dependents.GetItem(dependentName) != null) {
                return true;
            }
            return false;
        }
        #endregion
    }
}
