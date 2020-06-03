using System;
using Xap.Evaluation.Factory.Interfaces;
using Xap.Infrastructure.Exceptions;

namespace Xap.Evaluation.Factory.RuleSupport {
    internal static class XapRuleSyntax {
        internal static string PrepareRuleSyntax(string ruleSyntax,IXapRule rule) {
            try {
                foreach (IXapRuleVariable ruleVariable in rule.GetRuleVariables()) {
                    ruleSyntax = PrepareVariable(ruleSyntax,ruleVariable);
                    ruleSyntax = PrepareVariableAlias(ruleSyntax, ruleVariable);
                    ruleSyntax = PrepareGenericVariableValue(ruleSyntax, ruleVariable);
                    ruleSyntax = PrepareGenericVariableName(ruleSyntax, ruleVariable);

                    //clear the variable value to make sure it's reset for next run
                    ruleVariable.VariableValue = string.Empty;
                }
                return ruleSyntax;
            } catch (Exception ex) {
                throw new XapException($"Error preparing rule syntax for Rule:{rule.RuleName} Syntax:{rule.RuleSyntax}", ex);
            }
        }

        private static  string PrepareVariable(string syntax,IXapRuleVariable ruleVariable) {
            //search and replace based on class property name
            if (syntax.Contains($"'{ruleVariable.VariableName}'")) {
                ruleVariable.VariableValue = ReplaceEvaluationEngineReservedCharacters(ruleVariable.VariableValue);
                syntax = syntax.Replace(ruleVariable.VariableName, ruleVariable.VariableValue);
            }
            return syntax;
        }

        private static string PrepareVariableAlias(string syntax, IXapRuleVariable ruleVariable) {
            //search and replace based on class property alias name
            if (syntax.Contains($"'{ruleVariable.VariableAlias}'")) {
                ruleVariable.VariableValue = ReplaceEvaluationEngineReservedCharacters(ruleVariable.VariableValue);
                syntax = syntax.Replace(ruleVariable.VariableAlias, ruleVariable.VariableValue);
            }
            return syntax;
        }

        private static string PrepareGenericVariableValue(string syntax, IXapRuleVariable ruleVariable) {
            //search and replace for generic rules
            if (syntax.Contains("PROPERTY_VALUE")) {
                ruleVariable.VariableValue = ReplaceEvaluationEngineReservedCharacters(ruleVariable.VariableValue);
                syntax = syntax.Replace("PROPERTY_VALUE", ruleVariable.VariableValue);
            }
            return syntax;
        }

        private static string PrepareGenericVariableName(string syntax, IXapRuleVariable ruleVariable) {
            if (syntax.Contains("PROPERTY_NAME")) {
                ruleVariable.VariableValue = ReplaceEvaluationEngineReservedCharacters(ruleVariable.VariableValue);
                syntax = syntax.Replace("PROPERTY_NAME", ruleVariable.VariableValue);
            }
            return syntax;
        }

        private static string ReplaceEvaluationEngineReservedCharacters(string propertyValue) {
            propertyValue = propertyValue.Replace("'", "");
            propertyValue = propertyValue.Replace(":", "");
            propertyValue = propertyValue.Replace(";", "");
            propertyValue = propertyValue.Replace("/", ".");
            return propertyValue;
        }
    }
}
