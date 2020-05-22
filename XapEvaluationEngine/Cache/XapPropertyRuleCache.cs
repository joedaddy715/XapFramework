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
    public class XapPropertyRuleCache : XapCache<string, XapRuleCache> {
        private readonly object propItUpIllTakeIt = new object();

        #region "Constructors"

        private static readonly XapPropertyRuleCache instance = new XapPropertyRuleCache();

        static XapPropertyRuleCache() { }

        private XapPropertyRuleCache() { }

        public static XapPropertyRuleCache Instance {
            get { return instance; }
        }
        #endregion

        public XapRuleCache GetRuleCache(IXapRuleSearch ruleSearch,IXapDbContext dbContext) {
            XapRuleCache ruleCache = null;
            ruleCache = this.GetItem($"{ruleSearch.RuleType}.{ruleSearch.NameSpace}.{ruleSearch.PropertyName}");

            if (ruleCache != null) {
                return ruleCache.Clone();
            } else {
                ruleCache = XapRuleCache.Create();
            }

            dbContext.TSql = "CORE.SelectPropertyRules";

            IXapDataProvider db = DbFactory.Instance.XapDb(dbContext);

            string ruleDependents = string.Empty;

            try {
                XapDataReader dr = db.AddParameter(DbFactory.Instance.XapDbParameter("RuleType",ruleSearch.RuleType))
                   .AddParameter(DbFactory.Instance.XapDbParameter("PropertyName",ruleSearch.PropertyName))
                   .AddParameter(DbFactory.Instance.XapDbParameter("LobName",ruleSearch.LobName))
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
                    this.AddItem($"{ruleSearch.RuleType}.{ruleSearch.NameSpace}.{ruleSearch.PropertyName}", ruleCache);
                }

                return ruleCache.Clone();
            } catch (Exception ex) {
                XapLogger.Instance.Error($"Error loading rules for {ruleSearch.NameSpace}.{ruleSearch.PropertyName}");
                XapLogger.Instance.Write(ex.Message);
                throw;
            } finally {
                db.CloseConnection();
            }
        }
    }
}
