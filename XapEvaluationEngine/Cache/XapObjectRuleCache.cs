using System;
using Xap.Data.Factory;
using Xap.Evaluation.Engine.RuleSupport;
using Xap.Infrastructure.Caches;
using Xap.Infrastructure.Data;
using Xap.Infrastructure.Extensions;
using Xap.Infrastructure.Interfaces.Data;
using Xap.Infrastructure.Interfaces.Evaluation;
using Xap.Infrastructure.Logging;

namespace Xap.Evaluation.Engine.Cache {
    public class XapObjectRuleCache : XapCache<string, XapRuleCache> {
        private object propItUpIllTakeIt = new object();

        #region "Constructors"

        private static readonly XapObjectRuleCache instance = new XapObjectRuleCache();

        static XapObjectRuleCache() { }

        private XapObjectRuleCache() { }

        public static XapObjectRuleCache Instance {
            get { return instance; }
        }
        #endregion

        public XapRuleCache GetRuleCache(IXapRuleSearch ruleSearch,IXapDbContext dbContext) {
            XapRuleCache ruleCache = null;
            ruleCache = this.GetItem($"O.{ruleSearch.RuleType}.{ruleSearch.NameSpace}");

            if (ruleCache != null) {
                return ruleCache.Clone();
            } else {
                ruleCache = XapRuleCache.Create();
            }

            dbContext.TSql = "CORE.SelectRules";

            IXapDataProvider db = DbFactory.Instance.XapDb(dbContext);
            string ruleDependents = string.Empty;
            try {
                XapDataReader dr = db.AddParameter(DbFactory.Instance.XapDbParameter("RuleType",ruleSearch.RuleType))
                    .AddParameter(DbFactory.Instance.XapDbParameter("LobName",ruleSearch.LobName))
                    .AddParameter(DbFactory.Instance.XapDbParameter("ComponentName", ruleSearch.ComponentName))
                    .AddParameter(DbFactory.Instance.XapDbParameter("NameSpace",ruleSearch.NameSpace))
                    .ExecuteReader();

                while (dr.Read()) {
                    IXapRule _rule = XapRule.Create(dr.GetString("RuleName"));
                    _rule.RuleType = dr.GetString("RuleType");
                    _rule.RuleSyntax = dr.GetString("RuleSyntax");
                    _rule.RuleDescription = dr.GetString("RuleDesc");
                    _rule.RuleMessage = dr.GetString("RuleMessage");
                    _rule.PropertyName = dr.GetString("PropertyName");

                    ruleDependents = dr.GetString("Dependencies");

                    string[] dependents = ruleDependents.Split(',');
                    for (int i = 0; i < dependents.Length; i++) {
                        if (!_rule.HasDependent(dependents[i])) {
                            _rule.AddDependent(XapRuleDependent.Create(dependents[i]));
                        }
                    }

                    lock (propItUpIllTakeIt) {
                        ruleCache.AddRule(_rule.RuleName, _rule);
                    }
                }
                lock (propItUpIllTakeIt) {
                    this.AddItem($"O.{ruleSearch.RuleType}.{ruleSearch.NameSpace}", ruleCache);
                }
                return ruleCache.Clone();
            } catch (Exception ex) {
                XapLogger.Instance.Error($"Error loading rules for {ruleSearch.NameSpace}");
                XapLogger.Instance.Write(ex.Message);
                throw;
            } finally {
                db.CloseConnection();
            }
        }
    }
}
