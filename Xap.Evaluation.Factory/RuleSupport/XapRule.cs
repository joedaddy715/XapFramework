using System;
using System.Collections.Generic;
using System.Text;
using Xap.Evaluation.Engine.Evaluate;
using Xap.Evaluation.Engine.Parser;
using Xap.Evaluation.Factory.Interfaces;
using Xap.Infrastructure.Caches;
using Xap.Infrastructure.Exceptions;
using Xap.Infrastructure.Extensions;

namespace Xap.Evaluation.Factory.RuleSupport {
    internal class XapRule : IXapRule {
        #region "Constructors"
        private XapRule() { }

        private XapRule(string ruleName) {
            _ruleName = ruleName;
        }

        internal static IXapRule Create() {
            return new XapRule();
        }

        internal static IXapRule Create(string ruleName) {
            return new XapRule(ruleName);
        }
        #endregion

        #region "Properties"
        private XapCache<string, IXapRuleVariable> _ruleVariables = new XapCache<string, IXapRuleVariable>();
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

        private string _ruleValue = string.Empty;
        string IXapRule.RuleValue {
            get => _ruleValue;
            set => _ruleValue = value;
        }

        private string _syntaxError = string.Empty;
        string IXapRule.SyntaxError {
            get => _syntaxError;
        }
        #endregion

        #region "interface methods"
        bool IXapRule.EvaluateRule() {
            try {
                Token token = new Token(PrepareSyntax(_ruleSyntax));
                Evaluator eval = new Evaluator(token);

                if (!eval.Evaluate(out _ruleValue, out _syntaxError)) {
                    throw new Exception(_syntaxError);
                }
                return _ruleValue.ConvertValue<bool>();
            } catch (Exception ex) {
                throw new XapException($"Error evaluating rule {_ruleName}", ex);
            }
        }

        T IXapRule.EvaluateRule<T>() {
            try {
                Token token = new Token(PrepareSyntax(_ruleSyntax));
                Evaluator eval = new Evaluator(token);

                if (!eval.Evaluate(out _ruleValue, out _syntaxError)) {
                    throw new Exception(_syntaxError);
                }

                return _ruleValue.ConvertValue<T>();
            } catch (Exception ex) {
                throw new XapException($"Error evaluating rule {_ruleName}", ex);
            }
        }

        IEnumerable<IXapRuleVariable> IXapRule.GetRuleVariables() {
            foreach (KeyValuePair<string, IXapRuleVariable> kvp in _ruleVariables.GetItems()) {
                yield return kvp.Value;
            }
        }

        void IXapRule.AddRuleVariable(IXapRuleVariable ruleVariable) {
            _ruleVariables.AddItem(ruleVariable.VariableName,ruleVariable);
        }

        bool IXapRule.HasRuleVariable(string variableName) {
            if (_ruleVariables.GetItem(variableName) != null) {
                return true;
            }
            return false;
        }
        #endregion

        #region "private methods"
        //TODO:  remove the StringBuilder if rules aren't cached
        private string PrepareSyntax(string ruleSyntax) {
            if(_ruleVariables.Count == 0) {
                return ruleSyntax;
            }
            
            return XapRuleSyntax.PrepareRuleSyntax(new StringBuilder(ruleSyntax).ToString(), this);
        }
        #endregion
    }
}
