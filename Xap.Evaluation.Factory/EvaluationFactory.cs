using System;
using Xap.Evaluation.Factory.Interfaces;
using Xap.Evaluation.Factory.Providers;
using Xap.Evaluation.Factory.RuleSupport;
using Xap.Infrastructure.AppDomain;
using Xap.Infrastructure.Exceptions;

namespace Xap.Evaluation.Factory {
    public class EvaluationFactory {
        #region "Constructors"

        private static readonly EvaluationFactory instance = new EvaluationFactory();

        static EvaluationFactory() { }

        private EvaluationFactory() { }

        public static EvaluationFactory Instance {
            get { return instance; }
        }
        #endregion

        #region "public methods"
        public IXapRuleSet CreateRuleSet(string ruleSetName) {
            return XapRuleSet.Create(ruleSetName);
        }

        public IXapRule CreateRule() {
            return XapRule.Create();
        }

        public IXapRule CreateRule(string ruleName) {
            return XapRule.Create(ruleName);
        }

        public IXapRuleVariable CreateRuleVariable(string variableName) {
            return XapRuleVariable.Create(variableName);
        }

        public IXapRuleVariable CreateRuleVariable(string variableName,string variableValue) {
            return XapRuleVariable.Create(variableName,variableValue);
        }
        #endregion

        #region "Provider Loader"
        public IXapValidationProvider LoadValidationProvider(string providerType) {
            try {
                return AssemblyManager.Instance.CreateInstance<IXapValidationProvider>(providerType);
            } catch (Exception ex) {
                throw new XapException($"Error loading validation provider {providerType}", ex);
            }
        }
        #endregion
    }
}
