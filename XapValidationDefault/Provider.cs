using System;
using Xap.Data.Factory;
using Xap.Data.Factory.Attributes;
using Xap.Data.Factory.Interfaces;
using Xap.Evaluation.Engine.RuleSupport;
using Xap.Infrastructure.Core;
using Xap.Infrastructure.Exceptions;
using Xap.Infrastructure.Interfaces.Evaluation;
using Xap.Infrastructure.Interfaces.Security;
using Xap.Infrastructure.Interfaces.Validation;
using Xap.Infrastructure.Shared;
using Xap.Validation.Service;
using Xap.Logging.Factory;

namespace Xap.Validation.Default {
    public class Provider : IXapValidationProvider {
        #region "Constructors"
        public Provider() {

        }
        #endregion

        #region "Properties"
        IXapValidationService _validationService = null;
        IXapRuleSet ruleSet = null;
        IXapRule rule = null;
        IXapDataProvider db = null;

        #endregion

        IXapValidationService IXapValidationProvider.LoadRules<T>(T obj, IXapValidationService validationService,IXapUser xapUser,string ruleType) {
            _validationService = validationService ?? ValidationService.Create();
            try {
                string dbEnvironment = string.Empty;
                string lobName = string.Empty;
                string componentName = string.Empty;
                string ruleDependents = string.Empty;

                GetDbContext<T>(obj,out dbEnvironment,out lobName,out componentName);

                db = DbFactory.Instance.Db(dbEnvironment,lobName,"CORE.SelectRules");

                XapDataReader dr = db.AddParameter(DbFactory.Instance.DbParameter("RuleType",ruleType))
                    .AddParameter(DbFactory.Instance.DbParameter("LobName", lobName))
                    .AddParameter(DbFactory.Instance.DbParameter("ComponentName",componentName))
                    .AddParameter(DbFactory.Instance.DbParameter("NameSpace", obj.GetType().FullName))
                    .ExecuteReader();

                while (dr.Read()) {
                    ruleSet = validationService.AddRuleSet(dr.GetString("PropertyName"));
                    rule = ruleSet.CreateRule(dr.GetString("RuleName"));
                    rule.RuleType = dr.GetString("RuleType");
                    rule.RuleSyntax = dr.GetString("RuleSyntax");
                    rule.RuleDescription = dr.GetString("RuleDesc");
                    rule.RuleMessage = dr.GetString("RuleMessage");
                    rule.PropertyName = dr.GetString("PropertyName");

                    ruleDependents = dr.GetString("Dependencies");

                    string[] dependents = ruleDependents.Split(',');
                    for (int i = 0; i < dependents.Length; i++) {
                        if (!rule.HasDependent(dependents[i])) {
                            rule.AddDependent(XapRuleDependent.Create(dependents[i]));
                        }
                    }
                    ruleSet.AddRule(rule);
                }
                PrepareRuleSyntax<T>(obj, validationService,xapUser);
                return _validationService;
            }catch(XapException ex) {
                XapLogger.Instance.Error(ex.Message);
                throw;
            }catch(Exception ex) {
                XapLogger.Instance.Error(ex.Message);
                throw new XapException($"Error loading validation rules for {obj.GetType().FullName}", ex);
            } finally {
                db.CloseConnection();
            }
        }

        IXapValidationService IXapValidationProvider.LoadRules<T>(T obj,string propertyName, IXapValidationService validationService,IXapUser xapUser,string ruleType) {
            _validationService = validationService ?? ValidationService.Create();
            try {
                string dbEnvironment = string.Empty;
                string lobName = string.Empty;
                string componentName = string.Empty;
                string ruleDependents = string.Empty;

                GetDbContext<T>(obj, out dbEnvironment, out lobName, out componentName);

                db = DbFactory.Instance.Db(dbEnvironment, lobName, "CORE.SelectPropertyRules");

                XapDataReader dr = db.AddParameter(DbFactory.Instance.DbParameter("RuleType", ruleType))
                    .AddParameter(DbFactory.Instance.DbParameter("LobName", lobName))
                    .AddParameter(DbFactory.Instance.DbParameter("ComponentName", componentName))
                    .AddParameter(DbFactory.Instance.DbParameter("NameSpace", obj.GetType().FullName))
                    .AddParameter(DbFactory.Instance.DbParameter("PropertyName",propertyName))
                    .ExecuteReader();

                while (dr.Read()) {
                    ruleSet = validationService.AddRuleSet(dr.GetString("PropertyName"));
                    rule = ruleSet.CreateRule(dr.GetString("RuleName"));
                    rule.RuleType = dr.GetString("RuleType");
                    rule.RuleSyntax = dr.GetString("RuleSyntax");
                    rule.RuleDescription = dr.GetString("RuleDesc");
                    rule.RuleMessage = dr.GetString("RuleMessage");
                    rule.PropertyName = dr.GetString("PropertyName");

                    ruleDependents = dr.GetString("Dependencies");

                    string[] dependents = ruleDependents.Split(',');
                    for (int i = 0; i < dependents.Length; i++) {
                        if (!rule.HasDependent(dependents[i])) {
                            rule.AddDependent(XapRuleDependent.Create(dependents[i]));
                        }
                    }
                    ruleSet.AddRule(rule);
                }
                PrepareRuleSyntax<T>(obj, validationService,xapUser);
                return _validationService;
            } catch (XapException ex) {
                XapLogger.Instance.Error(ex.Message);
                throw;
            } catch (Exception ex) {
                XapLogger.Instance.Error(ex.Message);
                throw new XapException($"Error loading validation rules for {obj.GetType().FullName}", ex);
            } finally {
                db.CloseConnection();
            }
        }

        private void GetDbContext<T>(T obj,out string dbEnvironment,out string lobName, out string componentName) {
            DbExecution dbExecution = SharedMethods.GetCustomAttribute<DbExecution>(obj);
            if(dbExecution != null) {
                dbEnvironment = dbExecution.DbEnvironment;
            } else {
                throw new XapException($"DbEnvironment not found for {obj.GetType().FullName}");
            }

            lobName = (obj as XapObjectCore).Properties.GetProperty("LobName")?.GetValue(obj).ToString();
            if (string.IsNullOrEmpty(lobName)) {
                throw new XapException($"Db Connection name not found for {obj.GetType().FullName}");
            }

            componentName = (obj as XapObjectCore).Properties.GetProperty("ComponentName")?.GetValue(obj).ToString();
            if (string.IsNullOrEmpty(componentName)) {
                throw new XapException($"Component type not found for {obj.GetType().FullName}");
            }
        }

        private void PrepareRuleSyntax<T>(T obj,IXapValidationService validationService,IXapUser xapUser) {
            try {
                foreach (IXapRuleSet ruleSet in validationService.GetRuleSets()) {
                    foreach (IXapRule rule in ruleSet.GetRules()) {
                        rule.RuleSyntax = XapRuleSyntax.PrepareRuleSyntax((obj as XapObjectCore), rule, xapUser);
                    }
                }
            } catch (Exception ex) {
                XapLogger.Instance.Error(ex.Message);
                throw;
            }
        }
    }
}
