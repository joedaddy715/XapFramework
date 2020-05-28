using System;
using Xap.Data.Factory;
using Xap.Data.Factory.Interfaces;
using Xap.Evaluation.Engine.RuleSupport;
using Xap.Infrastructure.Caches;
using Xap.Infrastructure.Extensions;
using Xap.Infrastructure.Interfaces.Evaluation;
using Xap.Logging.Factory;

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

        public XapRuleCache GetRuleCache(IXapRuleSearch ruleSearch,IXapDbConnectionContext dbContext) {
            XapRuleCache ruleCache = null;
            ruleCache = this.GetItem($"O.{ruleSearch.RuleType}.{ruleSearch.NameSpace}");

            if (ruleCache != null) {
                return ruleCache.Clone();
            } else {
                ruleCache = XapRuleCache.Create();
            }

            dbContext.TSql = "CORE.SelectRules";

            IXapDataProvider db = DbFactory.Instance.Db(dbContext);
            string ruleDependents = string.Empty;
            try {
                XapDataReader dr = db.AddParameter(DbFactory.Instance.DbParameter("RuleType",ruleSearch.RuleType))
                    .AddParameter(DbFactory.Instance.DbParameter("LobName",ruleSearch.LobName))
                    .AddParameter(DbFactory.Instance.DbParameter("ComponentName", ruleSearch.ComponentName))
                    .AddParameter(DbFactory.Instance.DbParameter("NameSpace",ruleSearch.NameSpace))
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
