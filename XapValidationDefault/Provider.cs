using System;
using Xap.Data.Factory;
using Xap.Data.Factory.Attributes;
using Xap.Data.Factory.Interfaces;
using Xap.Evaluation.Factory;
using Xap.Evaluation.Factory.Interfaces;
using Xap.Evaluation.Factory.Providers;
using Xap.Evaluation.Factory.Services;
using Xap.Infrastructure.Core;
using Xap.Infrastructure.Exceptions;
using Xap.Infrastructure.Shared;

namespace Xap.Validation.Default {
    public class Provider : IXapValidationProvider {
        #region "Constructors"
        public Provider() {

        }
        #endregion

        #region "Properties"
        IXapEvaluationService evaluationService = XapEvaluationService.Create();
        #endregion

        IXapEvaluationService IXapValidationProvider.LoadRules<T>(T obj, string ruleType) {
            IXapDataProvider db = null;
            IXapRuleSet ruleSet = null;
            IXapRule rule = null;

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
                    ruleSet = evaluationService.GetRuleSet(dr.GetString("RuleSet"));
                    evaluationService.AddRuleSet(ruleSet);

                    rule = ruleSet.GetRule(dr.GetString("RuleName"));
                    rule.RuleType = dr.GetString("RuleType");
                    rule.RuleSyntax = dr.GetString("RuleSyntax");
                    rule.RuleDescription = dr.GetString("RuleDesc");
                    rule.RuleMessage = dr.GetString("RuleMessage");

                    IXapRuleVariable ruleVariable = EvaluationFactory.Instance.CreateRuleVariable(dr.GetString("PropertyName"));
                    rule.AddRuleVariable(ruleVariable);

                    ruleDependents = dr.GetString("Dependencies");

                    string[] dependents = ruleDependents.Split(',');
                    for (int i = 0; i < dependents.Length; i++) {
                        IXapRuleVariable variableDependent = EvaluationFactory.Instance.CreateRuleVariable(dependents[i]);
                        rule.AddRuleVariable(variableDependent);
                    }
                    ruleSet.AddRule(rule);
                }

                return evaluationService;
            }catch(XapException ex) {
                throw;
            }catch(Exception ex) {
                throw new XapException($"Error loading validation rules for {obj.GetType().FullName}", ex);
            } finally {
                db.CloseConnection();
            }
        }

        //IXapValidationService IXapValidationProvider.LoadRules<T>(T obj,string propertyName, IXapValidationService validationService,IXapUser xapUser,string ruleType) {
        //    _validationService = validationService ?? ValidationService.Create();
        //    try {
        //        string dbEnvironment = string.Empty;
        //        string lobName = string.Empty;
        //        string componentName = string.Empty;
        //        string ruleDependents = string.Empty;

        //        GetDbContext<T>(obj, out dbEnvironment, out lobName, out componentName);

        //        db = DbFactory.Instance.Db(dbEnvironment, lobName, "CORE.SelectPropertyRules");

        //        XapDataReader dr = db.AddParameter(DbFactory.Instance.DbParameter("RuleType", ruleType))
        //            .AddParameter(DbFactory.Instance.DbParameter("LobName", lobName))
        //            .AddParameter(DbFactory.Instance.DbParameter("ComponentName", componentName))
        //            .AddParameter(DbFactory.Instance.DbParameter("NameSpace", obj.GetType().FullName))
        //            .AddParameter(DbFactory.Instance.DbParameter("PropertyName",propertyName))
        //            .ExecuteReader();

        //        while (dr.Read()) {
        //            ruleSet = validationService.AddRuleSet(dr.GetString("PropertyName"));
        //            rule = ruleSet.CreateRule(dr.GetString("RuleName"));
        //            rule.RuleType = dr.GetString("RuleType");
        //            rule.RuleSyntax = dr.GetString("RuleSyntax");
        //            rule.RuleDescription = dr.GetString("RuleDesc");
        //            rule.RuleMessage = dr.GetString("RuleMessage");
        //            rule.PropertyName = dr.GetString("PropertyName");

        //            ruleDependents = dr.GetString("Dependencies");

        //            string[] dependents = ruleDependents.Split(',');
        //            for (int i = 0; i < dependents.Length; i++) {
        //                if (!rule.HasDependent(dependents[i])) {
        //                    rule.AddDependent(XapRuleDependent.Create(dependents[i]));
        //                }
        //            }
        //            ruleSet.AddRule(rule);
        //        }
        //        PrepareRuleSyntax<T>(obj, validationService,xapUser);
        //        return _validationService;
        //    } catch (XapException ex) {
        //        throw;
        //    } catch (Exception ex) {
        //        throw new XapException($"Error loading validation rules for {obj.GetType().FullName}", ex);
        //    } finally {
        //        db.CloseConnection();
        //    }
        //}

        private void GetDbContext<T>(T obj, out string dbEnvironment, out string lobName, out string componentName) {
            DbExecution dbExecution = SharedMethods.GetCustomAttribute<DbExecution>(obj);
            if (dbExecution != null) {
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
    }
}
