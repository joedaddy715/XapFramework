using System;
using System.Reflection;
using Xap.Infrastructure.Core;
using Xap.Infrastructure.Extensions;
using Xap.Infrastructure.Interfaces.Evaluation;
using Xap.Infrastructure.Interfaces.Security;
using Xap.Infrastructure.Logging;

namespace Xap.Evaluation.Engine.RuleSupport {
    //TODO: Test new property cache changes
    public class XapRuleSyntax {
        private static string PrepareRuleSyntax(XapObjectCore sourceObject, IXapRule rule) {
            try {
                string dependentProps = string.Empty;

                object propValue = null;
                object propName = null;

                //search and replace based on property Name
                foreach (PropertyInfo prop in sourceObject.Properties.GetProperties()) {

                    propName = prop.ShortName();
                    try {
                        propValue = prop.GetValue(sourceObject, null);
                    } catch {
                        propValue = string.Empty;
                    }

                    if (propValue == null) {
                        propValue = string.Empty;
                    }

                    propValue = ReplaceEvaluationEngineReservedCharacters(propValue.ToString());
                    //search and replace based on mapped property name
                    if (rule.RuleSyntax.Contains("'" + propName.ToString() + "'")) {
                        rule.RuleSyntax = rule.RuleSyntax.Replace(propName.ToString(), propValue.ToString());
                    }

                    if (rule.PropertyName == propName.ToString()) {
                        if (rule.RuleSyntax.Contains("PROPERTY_VALUE")) {
                            rule.RuleSyntax = rule.RuleSyntax.Replace("PROPERTY_VALUE", propValue.ToString());
                            rule.RuleMessage = rule.RuleMessage.Replace("PROPERTY_NAME", rule.PropertyName);
                            rule.RuleMessage = rule.RuleMessage.Replace("PROPERTY_VALUE", propValue.ToString());
                        }
                        if (rule.RuleSyntax.Contains("PROPERTY_NAME")) {
                            rule.RuleSyntax = rule.RuleSyntax.Replace("PROPERTY_NAME", propValue.ToString());
                            rule.RuleMessage = rule.RuleMessage.Replace("PROPERTY_NAME", rule.PropertyName);
                            rule.RuleMessage = rule.RuleMessage.Replace("PROPERTY_VALUE", propValue.ToString());
                        }
                    }

                    foreach (IXapRuleDependent dependent in rule.GetDependents()) {
                        PropertyInfo dependentProp = sourceObject.Properties.GetProperty(dependent.DependentName);
                        if (dependentProp != null) {
                            propName = prop.ShortName();

                            propValue = prop.GetValue(sourceObject, null).ToString();
                            propValue = ReplaceEvaluationEngineReservedCharacters(propValue.ToString());
                            if (rule.RuleSyntax.Contains(propName.ToString())) {
                                rule.RuleSyntax = rule.RuleSyntax.Replace(propName.ToString(), propValue.ToString());
                                rule.RuleMessage = rule.RuleMessage.Replace(propName.ToString(), propValue.ToString());
                            }
                        }
                    }
                }
                return rule.RuleSyntax;
            } catch (Exception ex) {
                XapLogger.Instance.Error($"Error preparing rule syntax for {sourceObject.GetType().Name}, Rule:{rule.RuleName} Syntax:{rule.RuleSyntax}");
                XapLogger.Instance.Write(ex.Message);
                throw;
            }
        }

        public static string PrepareRuleSyntax(XapObjectCore sourceObject, IXapRule rule, IXapUser xapUser) {
            try {
                rule.RuleSyntax = PrepareRuleSyntax(sourceObject, rule);
                if (xapUser != null) {
                    PropertyInfo[] props = xapUser.GetType().GetProperties(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
                    foreach (PropertyInfo prop in props) {
                        object userPropName = prop.ShortName();
                        object userPropValue = prop.GetValue(xapUser, null);

                        if (userPropValue == null) {
                            userPropValue = string.Empty;
                        }

                        userPropValue = ReplaceEvaluationEngineReservedCharacters(userPropValue.ToString());

                        if (rule.RuleSyntax.Contains("'" + userPropName.ToString() + "'")) {
                            rule.RuleSyntax = rule.RuleSyntax.Replace(userPropName.ToString(), userPropValue.ToString());
                        }
                    }
                }
                return rule.RuleSyntax;
            } catch (Exception ex) {
                XapLogger.Instance.Error($"Error preparing rule syntax for {sourceObject.GetType().Name}, Rule:{rule.RuleName} Syntax:{rule.RuleSyntax}");
                XapLogger.Instance.Write(ex.Message);
                throw;
            }
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
